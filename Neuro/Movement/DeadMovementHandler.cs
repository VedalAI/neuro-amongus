using System;
using System.Linq;
using Neuro.Events;
using Neuro.Extensions;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Movement;

[RegisterInIl2Cpp, FullShipStatusComponent]
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
            if (closestConsole.onlyFromBelow) target.y -= 0.5f; // TODO: this fails on divert power in the sensor room on mira if coming from above
            MoveToPosition(target);
            return;
        }

        switch (PlayerControl.LocalPlayer.Data.Role.Il2CppCastToTopLevel())
        {
            case GuardianAngelRole role: // TODO: Protect more smartly
                if (followPlayer is null || followPlayer.Data.IsDead)
                {
                    followPlayer = GetRandomAlivePlayer(false);
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
                if (followPlayer is null || followPlayer.Data.IsDead)
                {
                    followPlayer = GetRandomAlivePlayer(true);
                    Info($"Now following {followPlayer.name}");
                }

                MoveToPosition(followPlayer.GetTruePosition(), 1.0f);
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

        foreach (Console console in ConsoleFinder.GetOpenableConsoles(false))
        {
            float distance = Vector2.Distance(console.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestConsole = console;
            }
        }

        return closestConsole;
    }

    private static PlayerControl GetRandomAlivePlayer(bool impostors)
    {
        // We don't return impostors because there's no logic to avoid the killer or to stop following the target if they do something sus, so just pick a crewmate instaed. Guess it's fine to have PsychicNeuro sometimes ¯\_(ツ)_/¯
        return PlayerControl.AllPlayerControls._items
            .Where(p => p && !p.Data.Disconnected && !p.Data.IsDead && impostors == p.Data.Role.IsImpostor).Random();
    }
}