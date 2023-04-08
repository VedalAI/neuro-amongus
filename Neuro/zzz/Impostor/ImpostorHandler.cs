using UnityEngine;
using Reactor.Utilities.Attributes;
using System;
using System.Collections.Generic;
using BepInEx.Unity.IL2CPP.Utils;
using System.Collections;
using Il2CppInterop.Runtime.Attributes;
using System.Linq;

namespace Neuro.Impostor;

[RegisterInIl2Cpp]
public class ImpostorHandler : MonoBehaviour
{
    public ImpostorHandler(IntPtr ptr) : base(ptr) { }

    public List<PlainDoor> NearbyDoors { get; set; } = new List<PlainDoor>();

    public bool goingForKill { get; set; } = false;
    public PlayerControl killTarget { get; set; } = null;
    public bool attemptingVent { get; set; } = false;

    private void FixedUpdate()
    {
        if (!ShipStatus.Instance) return;
        if (MeetingHud.Instance) return;
        if (!PlayerControl.LocalPlayer) return;

        // TODO: Move UpdateNearbyDoors when implementing maps with doors that non-impostors can use
        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {

            // GetOrKillTarget();
            // AttemptVent();
        }
    }

    private void GetOrKillTarget(PlayerControl target = null)
    {
        if (PlayerControl.LocalPlayer.killTimer > 0f) return;

        if (killTarget == null && !PlayerControl.LocalPlayer.inVent)
        {
            if (target)
            {
                killTarget = target;
                return;
            }

            PlayerControl potentialKillTarget = null;

            foreach ((PlayerControl player, LastSeenPlayer lastSeen) in NeuroPlugin.Instance.Vision.PlayerLocations)
            {
                // ignore ourselves, dead players, and other impostors
                if (player.AmOwner) continue;
                if (player.Data.IsDead) continue;
                if (player.Data.Role.IsImpostor) continue;

                if (lastSeen.time > Time.timeSinceLevelLoad - (2 * Time.fixedDeltaTime))
                {
                    // if there are multiple players in view, avoid trying to kill so we dont give ourselves up
                    if (potentialKillTarget != null)
                    {
                        potentialKillTarget = null;
                        break;
                    }
                    potentialKillTarget = player;
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
        Info($"I just killed {killTarget.Data.PlayerName}!");
        goingForKill = false;
        killTarget = null;
        NeuroPlugin.Instance.Tasks.UpdatePathToTask(NeuroPlugin.Instance.Tasks.GetFurthestTask());
    }


    [HideFromIl2Cpp]
    private void AttemptVent()
    {
        // currently will just enter a vent whenever possible
        // this should be changed to be more situational
        if (attemptingVent && ClosestVent != null && !PlayerControl.LocalPlayer.inVent && !PlayerControl.LocalPlayer.walkingToVent)
        {
            if (HudManager.Instance.ImpostorVentButton.currentTarget.Id == ClosestVent.Id)
            {
                // vent.EnterVent() and vent.Use() dont actually put you in the vent for whatever reason so just click the button virtually
                HudManager.Instance.ImpostorVentButton.DoClick();
            }
        }
    }

    public IEnumerator CoStartVentOut(Vent original)
    {
        Info("I entered a vent!");
        List<Vent> possibleVents = GetAvailableNearbyVents(original);
        Vent current = possibleVents[UnityEngine.Random.RandomRangeInt(0, possibleVents.Count)];
        yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
        if (!original.TryMoveToVent(current, out string error))
        {
            Error($"Failed to move to vent {current.Id}, reason: {error}");
        }
        while (true)
        {
            // use a random time between vent moves to make it more realistic
            yield return new WaitForSeconds(UnityEngine.Random.RandomRange(0.8f, 1.2f));
            bool playerFound = false;
            foreach ((PlayerControl player, LastSeenPlayer lastSeen) in NeuroPlugin.Instance.Vision.PlayerLocations)
            {
                if (player.AmOwner) continue;
                if (player.Data.IsDead) continue;
                if (player.Data.Role.IsImpostor) continue;

                // if we see a player in our radius, try a different vent
                if (lastSeen.time > Time.timeSinceLevelLoad - (2 * Time.fixedDeltaTime))
                {
                    Info($"Spotted {player.name}, trying a different exit vent...");
                    Vent next = GetAvailableNearbyVents(current)[UnityEngine.Random.RandomRangeInt(0, possibleVents.Count)];
                    if (!current.TryMoveToVent(next, out error))
                    {
                        Error($"Failed to move to vent {next.Id}, reason: {error}");
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
                attemptingVent = false;
                yield break;
            }
        }
    }

    // since some vents can have a variable amount of neighbors, use this helper method to get available ones
    private List<Vent> GetAvailableNearbyVents(Vent vent) => vent.NearbyVents.Where(v => v).ToList();

    public void ResetAfterMeeting()
    {
        killTarget = null;
        goingForKill = false;
    }
}
