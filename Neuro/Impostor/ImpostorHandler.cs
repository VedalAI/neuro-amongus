using UnityEngine;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using Neuro.Vision.DataStructures;
using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;

namespace Neuro.Impostor;

[RegisterInIl2Cpp]
public class ImpostorHandler : MonoBehaviour
{
    public ImpostorHandler(IntPtr ptr) : base(ptr) { }

    public bool isImpostor { get; set; } = false;
    public bool goingForKill { get; set; } = false;
    public PlayerControl killTarget { get; set; } = null;
    public Vent closestVent { get; set; } = null;
    // initalized in ShipStatus_Awake
    public List<Vent> vents = new List<Vent>();

    // vent cooldown for testing purposes so we dont just instantly jump back into the vent we exited
    // can be removed once logic is implemented
    public int ventCooldown { get; set; } = 0;

    // TODO: move this + related logic to Vision if necessary
    public Vector2 DirectionToNearestVent { get; set; } = Vector2.zero;

    public void FixedUpdate()
    {
        if (!ShipStatus.Instance) return;
        if (MeetingHud.Instance) return;
        if (!PlayerControl.LocalPlayer) return;

        // TODO: maybe try and make this only update once
        isImpostor = PlayerControl.LocalPlayer.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor;

        if (isImpostor)
        {
            UpdateNearestVent();
            GetOrKillTarget();
            AttemptVent();
            if (ventCooldown > 0)
            {
                ventCooldown--;
            }
        }
    }

    [HideFromIl2Cpp]
    private void UpdateNearestVent()
    {
        Vent closest = null;
        float closestDistance = 9999f;
        foreach (Vent vent in vents)
        {
            float distance = Vector2.Distance(vent.transform.position, PlayerControl.LocalPlayer.transform.position);
            if (distance < closestDistance)
            {
                closest = vent;
                closestDistance = distance;
            }
        }
        closestVent = closest;
        if (closestVent == null)
        {
            Warning("Closest vent was null this fixedupdate!");
            return;
        }
        DirectionToNearestVent = (closestVent.transform.position - PlayerControl.LocalPlayer.transform.position).normalized;
    }

    [HideFromIl2Cpp]
    private void GetOrKillTarget()
    {
        if (PlayerControl.LocalPlayer.killTimer > 0f) return;

        if (killTarget == null && !PlayerControl.LocalPlayer.inVent)
        {
            PlayerControl potentialKillTarget = null;

            foreach (KeyValuePair<PlayerControl, LastSeenPlayer> player in NeuroPlugin.Instance.Vision.GetPlayerLocations())
            {
                // ignore ourselves, dead players, and other impostors
                if (PlayerControl.LocalPlayer == player.Key) continue;
                if (player.Key.Data.IsDead) continue;
                if (player.Key.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor) continue;

                if (player.Value.time > Time.timeSinceLevelLoad - (2 * Time.fixedDeltaTime))
                {
                    // if there are multiple players in view, avoid trying to kill so we dont give ourselves up
                    if (potentialKillTarget != null)
                    {
                        potentialKillTarget = null;
                        break;
                    }
                    potentialKillTarget = player.Key;
                }
            }
            // if the target ends up being the only target in the room, mark them a kill target and pathfind to them
            if (potentialKillTarget != null)
            {
                killTarget = potentialKillTarget;
            }
        }
        else
        {
            if (!goingForKill && HudManager.Instance.KillButton.currentTarget != null)
            {
                goingForKill = true;
                PlayerControl.LocalPlayer.StartCoroutine(MurderTarget());
            }
        }
    }

    [HideFromIl2Cpp]
    private IEnumerator MurderTarget()
    {
        // wait a random amount of time so we dont just instantly kill them the second we are in range
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.2f, 0.6f));
        // this actually works in practice area which is pretty nice
        PlayerControl.LocalPlayer.MurderPlayer(killTarget);
        Info(String.Format("I just killed {0}!", killTarget.Data.PlayerName));
        goingForKill = false;
        killTarget = null;
        NeuroPlugin.Instance.Tasks.UpdatePathToTask(NeuroPlugin.Instance.Tasks.GetFurthestTask());
    }

    [HideFromIl2Cpp]
    private void AttemptVent()
    {
        // currently will just enter a vent whenever possible
        // this should be changed to be more situational
        if (closestVent != null && ventCooldown == 0 && !PlayerControl.LocalPlayer.inVent && !PlayerControl.LocalPlayer.walkingToVent)
        {
            if (HudManager.Instance.ImpostorVentButton.currentTarget == closestVent)
            {
                // vent.EnterVent() and vent.Use() dont actually put you in the vent for whatever reason so just click the button virtually
                HudManager.Instance.ImpostorVentButton.DoClick();
            }
        }
    }

    public IEnumerator Vent(Vent original)
    {
        Info("I entered a vent!");
        List<Vent> possibleVents = GetAvailableNearbyVents(original);
        Vent current = possibleVents[UnityEngine.Random.RandomRangeInt(0, possibleVents.Count)];
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
        string error;
        if (!original.TryMoveToVent(current, out error))
        {
            Error(String.Format("Failed to move to vent {0}, reason: {1}", current.Id, error));
        }
        while (true)
        {
            // use a random time between vent moves to make it more realistic
            yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
            bool playerFound = false;
            foreach (KeyValuePair<PlayerControl, LastSeenPlayer> player in NeuroPlugin.Instance.Vision.GetPlayerLocations())
            {
                if (PlayerControl.LocalPlayer == player.Key) continue;
                if (player.Key.Data.IsDead) continue;
                if (player.Key.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor) continue;

                // if we see a player in our radius, try a different vent
                if (player.Value.time > Time.timeSinceLevelLoad - (2 * Time.fixedDeltaTime))
                {
                    Info(String.Format("Spotted {0}, trying a different exit vent...", player.Key.name));
                    Vent next;
                    while (true)
                    {
                        possibleVents = GetAvailableNearbyVents(current);
                        next = possibleVents[UnityEngine.Random.RandomRangeInt(0, possibleVents.Count)];
                        if (current == next) continue;
                        break;
                    }
                    if (!current.TryMoveToVent(next, out error))
                    {
                        Error(String.Format("Failed to move to vent {0}, reason: {1}", next.Id, error));
                    }
                    current = next;
                    playerFound = true;
                    break;
                }
            }
            // if we dont see anyone, exit the vent we're currently in
            // also reset the kill target out of the vent to prevent any crossmap targeting
            if (!playerFound)
            {
                HudManager.Instance.ImpostorVentButton.DoClick();
                killTarget = null;
                ventCooldown = 60;
                yield break;
            }
        }
    }

    // since some vents can have a variable amount of neighbors, use this helper method to get available ones
    private List<Vent> GetAvailableNearbyVents(Vent vent)
    {
        List<Vent> result = new List<Vent>();
        if (vent.Left)
            result.Add(vent.Left);
        if (vent.Center)
            result.Add(vent.Center);
        if (vent.Right)
            result.Add(vent.Right);
        return result;
    }

    public void ResetAfterMeeting()
    {
        killTarget = null;
        goingForKill = false;
    }
}

