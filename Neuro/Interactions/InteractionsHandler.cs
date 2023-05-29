﻿using System;
using System.Diagnostics;
using Neuro.Events;
using Neuro.Minigames;
using Neuro.Minigames.Solvers;
using Neuro.Movement;
using Neuro.Recording.LocalPlayer;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Interactions;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class InteractionsHandler : MonoBehaviour
{
    public static InteractionsHandler Instance { get; private set; }

    public InteractionsHandler(IntPtr ptr) : base(ptr)
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

    [Conditional("FULL")]
    public void UseTarget(IUsable usable)
    {
        if (MeetingHud.Instance || Minigame.Instance || usable == null) return;

        // TODO: Allow neural network to specifiy intention of interacting with usables
        switch (usable.Il2CppCastToTopLevel())
        {
            case Console console:
                UseConsole(console);
                break;

            case DeconControl decon:
                LocalPlayerRecorder.Instance.RecordInteract();
                decon.Use();
                break;

            case DoorConsole door:
                LocalPlayerRecorder.Instance.RecordInteract();
                door.Use();
                break;

            case SystemConsole system:
                UseSystemConsole(system);
                break;

            case Ladder ladder:
                if (ladder.Destination.transform.position.y > ladder.transform.position.y && MovementHandler.Instance.ForcedMoveDirection.y == 1.0f) {
                    ladder.Use();
                } else if (ladder.Destination.transform.position.y < ladder.transform.position.y && MovementHandler.Instance.ForcedMoveDirection.y == -1.0f) {
                    ladder.Use();
                } 
                break;

            // case MapConsole admin:
            //     break;

            // case OpenDoorConsole simpleDoor:
            //     break;

            case PlatformConsole platform:
                if (platform.Platform.IsLeft && MovementHandler.Instance.ForcedMoveDirection.x == 1.0f) {
                    platform.Use();
                } else if(!platform.Platform.IsLeft && MovementHandler.Instance.ForcedMoveDirection.x == -1.0f) {
                    platform.Use();
                }
                break;



            // case Vent vent:
            //     break;
        }
    }

    private bool isPlayerFacingPosition(Vector2 position, float angle=45) {
        if (MovementHandler.Instance.ForcedMoveDirection == Vector2.zero) return false;
        if (Vector2.Angle(MovementHandler.Instance.ForcedMoveDirection, position - (Vector2)PlayerControl.LocalPlayer.transform.position) < angle) {
            return true;
        }
        return false;
    }

    public void UseConsole(Console console)
    {
        PlayerTask task = console.FindTask(PlayerControl.LocalPlayer);
        if (task == null)
        {
            Warning($"Unable to find task from console id {console.ConsoleId}");
            return;
        }

        Minigame minigame = task.GetMinigamePrefab();

        if (MinigameHandler.ShouldOpenConsole(console, minigame, task))
        {
            LocalPlayerRecorder.Instance.RecordInteract();
            console.Use();
        }
        else
        {
            Warning($"Shouldn't open console id {console.ConsoleId} for minigame {task.GetMinigamePrefab().GetIl2CppType().Name}");
        }
    }

    public void UseSystemConsole(SystemConsole console)
    {
        if (console.MinigamePrefab.TryCast<EmergencyMinigame>())
        {
            if (EmergencySolver.ShouldOpenEmergency())
            {
                LocalPlayerRecorder.Instance.RecordInteract();
                console.Use();
            }
            else
            {
                Warning("Shouldn't open emergency button");
            }
        }
        else
        {
            // as stated in EmergencySolver, we currently have no plans to open SystemConsoles besides the emergency button
            // so ignore everything else
            Warning($"Ignoring non-emergency button console");
        }
    }
}
