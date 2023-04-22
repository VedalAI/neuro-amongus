using System;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Movement;

[RegisterInIl2Cpp]
public sealed class DeadMovementHandler : MonoBehaviour
{
    private PlayerControl followPlayer = null;

    public HauntMenuMinigame minigame { get; set; }

    public static DeadMovementHandler Instance { get; private set; }

    public DeadMovementHandler(IntPtr ptr) : base(ptr)
    {
    }

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }
        Instance = this;
    }


    public void Move() 
    {
        Console closestConsole = getClosestConsole();
        if (closestConsole) {
            Vector2 target = closestConsole.transform.position;
            if (closestConsole.onlyFromBelow) target.y -= 0.5f;
            moveToPosition(target);
            return;
        }

        switch(PlayerControl.LocalPlayer.Data.RoleType)
        {
            case RoleTypes.CrewmateGhost:
            case RoleTypes.ImpostorGhost:
                // TODO: sabotage when impostor
                MovementHandler.Instance.ForcedMoveDirection = Vector2.zero;
                PlayerControl.LocalPlayer.Data.Role.UseAbility();
                break;

            case RoleTypes.GuardianAngel:
                // does not have the ability to haunt so we follow manually
                if (followPlayer is null || followPlayer.Data.IsDead)
                {
                    followPlayer = getRandomAlivePlayer();
                    Info("Now following " + followPlayer.name);
                }
                moveToPosition(followPlayer.GetTruePosition(), 1.0f);
                break;

            default:
                MovementHandler.Instance.ForcedMoveDirection = Vector2.zero;
                Warning($"Cannot handle movement for {PlayerControl.LocalPlayer.Data.RoleType}");
                break;
        }
    }

    private void moveToPosition(Vector2 target, float margin = 0.1f)
    {
        if (Vector2.Distance(target, PlayerControl.LocalPlayer.GetTruePosition()) < margin) MovementHandler.Instance.ForcedMoveDirection = Vector2.zero;
        else MovementHandler.Instance.ForcedMoveDirection = (target - PlayerControl.LocalPlayer.GetTruePosition()).normalized;
        
    }

    private Console getClosestConsole()
    {
        Console closestConsole = null;
        float closestDistance = 999f;
        
        foreach (NormalPlayerTask task in PlayerControl.LocalPlayer.myTasks.ToArray().OfIl2CppType<NormalPlayerTask>().Where(t => !t.IsComplete))
        {
            foreach (Console console in task.FindConsoles())
            {
                if (!console) continue;
                var distance = Vector2.Distance(console.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestConsole = console;
                }
            }
        }
        return closestConsole;
    }

    private PlayerControl getRandomAlivePlayer() 
    {
        List<PlayerControl> alivePlayers = new();
        foreach (PlayerControl player in PlayerControl.AllPlayerControls) 
        {
            if (!player.Data.IsDead) 
            {
                alivePlayers.Add(player);
            } 
        }
        System.Random random = new();
        int i = random.Next(alivePlayers.Count);
        return alivePlayers[i];
    }

    [EventHandler(EventTypes.GameStarted)]
    public static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<DeadMovementHandler>();
    }
}
