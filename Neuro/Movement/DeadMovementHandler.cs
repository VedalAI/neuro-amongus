using System;
using System.Linq;
using Neuro.Events;
using Neuro.Minigames;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Movement;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class DeadMovementHandler : MonoBehaviour
{
    private PlayerControl followPlayer;

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
        Console closestConsole = GetClosestConsole();
        if (closestConsole)
        {
            Vector2 target = closestConsole.transform.position;
            if (closestConsole.onlyFromBelow) target.y -= 0.5f;
            MoveToPosition(target);
            return;
        }

        switch (PlayerControl.LocalPlayer.Data.Role.Il2CppCastToTopLevel())
        {
            case GuardianAngelRole role:
                if (followPlayer is null || followPlayer.Data.IsDead)
                {
                    followPlayer = GetRandomAlivePlayer();
                    Info($"Now following {followPlayer.name}");
                }

                if (MoveToPosition(followPlayer.GetTruePosition(), 1.0f) && !role.IsCoolingDown)
                {
                    role.SetPlayerTarget(followPlayer);
                    role.UseAbility();
                    Info($"Protected {followPlayer.name}");
                }
                break;

            case CrewmateGhostRole:
            case ImpostorGhostRole:
                MovementHandler.Instance.ForcedMoveDirection = Vector2.zero;
                PlayerControl.LocalPlayer.Data.Role.UseAbility();
                break;

            default:
                MovementHandler.Instance.ForcedMoveDirection = Vector2.zero;
                Warning($"Cannot handle movement for {PlayerControl.LocalPlayer.Data.RoleType}");
                break;
        }
    }

    private static bool MoveToPosition(Vector2 target, float margin = 0.1f)
    {
        if (Vector2.Distance(target, PlayerControl.LocalPlayer.GetTruePosition()) < margin)
        {
            MovementHandler.Instance.ForcedMoveDirection = Vector2.zero;
            return true;
        }
        else
        {
            MovementHandler.Instance.ForcedMoveDirection = (target - PlayerControl.LocalPlayer.GetTruePosition()).normalized;
            return false;
        }
    }

    private static Console GetClosestConsole()
    {
        Console closestConsole = null;
        float closestDistance = 999f;

        foreach (NormalPlayerTask task in PlayerControl.LocalPlayer.myTasks.ToArray().OfIl2CppType<NormalPlayerTask>().Where(t => !t.IsComplete))
        {
            foreach (Console console in task.FindConsoles()._items.Where(c => c && MinigameHandler.ShouldOpenConsole(c, task.MinigamePrefab, task)))
            {
                float distance = Vector2.Distance(console.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestConsole = console;
                }
            }
        }

        return closestConsole;
    }

    private static PlayerControl GetRandomAlivePlayer()
    {
        // We don't return impostors because there's no logic to avoid the killer or to stop following the target if they do something sus, so just pick a crewmate instaed. Guess it's fine to have PsychicNeuro sometimes ¯\_(ツ)_/¯
        return PlayerControl.AllPlayerControls._items.Where(p => p && !p.Data.Disconnected && !p.Data.IsDead && !p.Data.Role.IsImpostor).Random();
    }
}
