using System;
using Neuro.Events;
using Neuro.Minigames;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Interactions;

[RegisterInIl2Cpp]
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

    public void UseTarget(IUsable usable)
    {
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        // TODO: Allow neural network to specifiy intention of interacting with usables
        switch (usable.Cast<Il2CppSystem.Object>().Il2CppCastToTopLevel())
        {
            case Console console:
                UseConsole(console);
                break;

            case DeconControl decon:
                decon.Use();
                break;

            case DoorConsole door:
                door.Use();
                break;

            // case Ladder ladder:
            //     break;

            // case MapConsole admin:
            //     break;

            // case OpenDoorConsole simpleDoor:
            //     break;

            // case PlatformConsole platform:
            //     break;

            // case SystemConsole system:
            //     break;

            // case Vent vent:
            //     break;
        }
    }

    public void UseConsole(Console console)
    {
        PlayerTask task = console.FindTask(PlayerControl.LocalPlayer);
        MinigameSolver solver = MinigameHandler.GetMinigameSolver(task.GetMinigamePrefab());
        if (solver == null) return;

        if (solver.CanUseConsole(console, task))
        {
            console.Use();
        }
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<InteractionsHandler>();
    }
}
