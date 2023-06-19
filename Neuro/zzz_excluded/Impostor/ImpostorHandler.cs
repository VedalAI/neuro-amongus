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

    public bool goingForKill { get; set; } = false;
    public PlayerControl killTarget { get; set; } = null;

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

    public void ResetAfterMeeting()
    {
        killTarget = null;
        goingForKill = false;
    }
}
