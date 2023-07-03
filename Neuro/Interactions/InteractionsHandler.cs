using System.Diagnostics;
using Neuro.Extensions;
using Neuro.Minigames;
using Neuro.Minigames.Solvers;
using Neuro.Movement;
using Neuro.Recording.LocalPlayer;
using UnityEngine;

namespace Neuro.Interactions;

public static class InteractionsHandler
{
    [Conditional("FULL")]
    public static void UseTarget(IUsable usable)
    {
        if (MeetingHud.Instance || Minigame.Instance || usable == null) return;

        switch (usable.Il2CppCastToTopLevel())
        {
            // Consoles

            case Console console:
                UseConsole(console);
                break;

            case SystemConsole system: // TODO: Should we check vitals? CAMS??
                UseSystemConsole(system);
                break;

            // Doors

            case DoorConsole door:
                UseDoor(door);
                break;

            case DeconControl decon:
                UseDeconDoor(decon);
                break;

            case OpenDoorConsole simpleDoor:
                UseSimpleDoor(simpleDoor);
                break;

            // Transportation

            case Ladder ladder:
                UseLadder(ladder);
                break;

            case PlatformConsole platform:
                UsePlatform(platform);
                break;
        }
    }

    private static void UseConsole(Console console)
    {
        PlayerTask task = console.FindTask(PlayerControl.LocalPlayer);
        if (task == null)
        {
            // Warning($"Unable to find task from console id {console.ConsoleId}");
            return;
        }

        Minigame minigame = task.GetMinigamePrefab();

        if (MinigameHandler.ShouldOpenConsole(console, minigame, task))
        {
            Interact(console);
        }
        else
        {
            // Warning($"Shouldn't open console id {console.ConsoleId} for minigame {task.GetMinigamePrefab().GetIl2CppType().Name}");
        }
    }

    private static void UseSystemConsole(SystemConsole console)
    {
        if (console.MinigamePrefab.TryCast<EmergencyMinigame>())
        {
            if (EmergencySolver.ShouldOpenEmergency())
            {
                Interact(console);
            }
            else
            {
                // Warning("Shouldn't open emergency button");
            }
        }
        else
        {
            // as stated in EmergencySolver, we currently have no plans to open SystemConsoles besides the emergency button
            // so ignore everything else
            // Warning($"Ignoring non-emergency button console");
        }
    }

    private static void UseDoor(DoorConsole console)
    {
        Vector2 velocity = PlayerControl.LocalPlayer.MyPhysics.Velocity;
        bool isHorizontal = console.MyDoor.myCollider.size.x > console.MyDoor.myCollider.size.y;

        Vector2 consolePos = console.transform.position;
        Vector2 playerPos = PlayerControl.LocalPlayer.transform.position;

        // horizontal door
        if (isHorizontal && velocity.y == 0)
        {
            if (consolePos.y > playerPos.y && MovementHandler.Instance.ForcedMoveDirection.y == 1.0f)
            {
                Interact(console);
            }
            else if (consolePos.y < playerPos.y && MovementHandler.Instance.ForcedMoveDirection.y == -1.0f)
            {
                Interact(console);
            }
        }

        // vertical door
        if (!isHorizontal && velocity.x == 0)
        {
            if (consolePos.x > playerPos.x && MovementHandler.Instance.ForcedMoveDirection.x == 1.0f)
            {
                Interact(console);
            }
            else if (consolePos.x < playerPos.x && MovementHandler.Instance.ForcedMoveDirection.x == -1.0f)
            {
                Interact(console);
            }
        }
    }

    private static void UseDeconDoor(DeconControl console)
    {
        Vector2 velocity = PlayerControl.LocalPlayer.MyPhysics.Velocity;

        bool isUpperDoor = console.OnUse.m_PersistentCalls.m_Calls._items[0].arguments.boolArgument;
        PlainDoor targetDoor = (isUpperDoor ? console.System.UpperDoor : console.System.LowerDoor).Cast<PlainDoor>();
        bool isHorizontal = targetDoor.myCollider.size.x > targetDoor.myCollider.size.y;

        Vector2 consolePos = targetDoor.transform.position;
        Vector2 playerPos = PlayerControl.LocalPlayer.transform.position;

        // horizontal door
        if (isHorizontal && velocity.y == 0)
        {
            if (consolePos.y > playerPos.y && MovementHandler.Instance.ForcedMoveDirection.y == 1.0f)
            {
                Interact(console);
            }
            else if (consolePos.y < playerPos.y && MovementHandler.Instance.ForcedMoveDirection.y == -1.0f)
            {
                Interact(console);
            }
        }

        // vertical door
        if (!isHorizontal && velocity.x == 0)
        {
            if (consolePos.x > playerPos.x && MovementHandler.Instance.ForcedMoveDirection.x == 1.0f)
            {
                Interact(console);
            }
            else if (consolePos.x < playerPos.x && MovementHandler.Instance.ForcedMoveDirection.x == -1.0f)
            {
                Interact(console);
            }
        }
    }

    private static void UseSimpleDoor(OpenDoorConsole console)
    {
        Vector2 velocity = PlayerControl.LocalPlayer.MyPhysics.Velocity;
        bool isHorizontal = console.MyDoor.myCollider.size.x > console.MyDoor.myCollider.size.y;

        Vector2 consolePos = console.transform.position;
        Vector2 playerPos = PlayerControl.LocalPlayer.transform.position;

        // horizontal door
        if (isHorizontal && velocity.y == 0)
        {
            if (consolePos.y > playerPos.y && MovementHandler.Instance.ForcedMoveDirection.y == 1.0f)
            {
                Interact(console);
            }
            else if (consolePos.y < playerPos.y && MovementHandler.Instance.ForcedMoveDirection.y == -1.0f)
            {
                Interact(console);
            }
        }

        // vertical door
        if (!isHorizontal && velocity.x == 0)
        {
            if (consolePos.x > playerPos.x && MovementHandler.Instance.ForcedMoveDirection.x == 1.0f)
            {
                Interact(console);
            }
            else if (consolePos.x < playerPos.x && MovementHandler.Instance.ForcedMoveDirection.x == -1.0f)
            {
                Interact(console);
            }
        }
    }

    private static void UseLadder(Ladder console)
    {
        if (PlayerControl.LocalPlayer.MyPhysics.Velocity.y != 0) return;

        Vector2 destinationPos = console.Destination.transform.position;
        Vector2 currentPos = console.transform.position;

        if (destinationPos.y > currentPos.y && MovementHandler.Instance.ForcedMoveDirection.y == 1.0f)
        {
            Interact(console);
        }

        if (destinationPos.y < currentPos.y && MovementHandler.Instance.ForcedMoveDirection.y == -1.0f)
        {
            Interact(console);
        }
    }

    private static void UsePlatform(PlatformConsole console)
    {
        if (PlayerControl.LocalPlayer.MyPhysics.Velocity.x != 0) return;

        if (console.Platform.IsLeft && MovementHandler.Instance.ForcedMoveDirection.x == 1.0f)
        {
            Interact(console);
        }

        if (!console.Platform.IsLeft && MovementHandler.Instance.ForcedMoveDirection.x == -1.0f)
        {
            Interact(console);
        }
    }

    private static void Interact(MonoBehaviour usable)
    {
        LocalPlayerRecorder.Instance.RecordInteract();
        usable.Cast<IUsable>().Use();
    }
}