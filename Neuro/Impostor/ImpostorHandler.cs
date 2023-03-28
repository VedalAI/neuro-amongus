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

    // TODO: move this + related logic to Vision if necessary
    public Vector2 DirectionToNearestVent { get; set; } = Vector2.zero;

    public void FixedUpdate()
    {
        if (!ShipStatus.Instance) return;
        if (MeetingHud.Instance) return;

        // TODO: maybe try and make this only update once
        isImpostor = PlayerControl.LocalPlayer.Data.RoleType == AmongUs.GameOptions.RoleTypes.Impostor;

        if (isImpostor)
        {
            UpdateNearestVent();
            GetOrKillTarget();
            AttemptVent();
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
        if (closestVent == null)
        {
            Info("closestVent is null, falling back to doing tasks!");
            NeuroPlugin.Instance.Tasks.UpdatePathToTask(NeuroPlugin.Instance.Tasks.GetFurthestTask());
        }
    }

    [HideFromIl2Cpp]
    private void AttemptVent()
    {
        // currently will just enter a vent whenever possible
        // this should be changed to be more situational
        if (closestVent != null && !PlayerControl.LocalPlayer.inVent && !PlayerControl.LocalPlayer.walkingToVent)
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
        Vent current = original.NearbyVents[UnityEngine.Random.RandomRangeInt(0, original.NearbyVents.Count)];
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
        original.MoveToVent(current);
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
                        next = current.NearbyVents[UnityEngine.Random.RandomRangeInt(0, current.NearbyVents.Count)];
                        if (current == next) continue;
                        break;
                    }
                    current.MoveToVent(next);
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
                yield break;
            }
        }
    }

    [HideFromIl2Cpp]
    public void ResetAfterMeeting()
    {
        killTarget = null;
        goingForKill = false;
    }
}

