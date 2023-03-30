using System;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using Neuro.Minigames.Completion;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neuro.Minigames;

public static class MinigameHandler
{
    public static void CompleteMinigame(Minigame minigame, PlayerTask task)
    {
        // TODO: Make sure this works with the safe minigame on airship
        // TODO: Make sure this works with the simon says minigame on skeld

        if (minigame.TryCast<IDoorMinigame>() is { })
        {
            minigame.StartCoroutine(CompleteDoorMinigame(minigame));
            return;
        }

        // If task is null, then the minigame can be one of the following:
        // - Door minigame (handled above)
        // - Spawn in minigame
        // - Role ability minigame
        // - System console minigame
        if (task!?.TryCast<NormalPlayerTask>() is not { } normalPlayerTask) return;

        minigame.StartCoroutine(CompleteTaskMinigame(minigame, normalPlayerTask));
    }

    private static IEnumerator CompleteTaskMinigame(Minigame minigame, NormalPlayerTask task)
    {
        if (MinigameSolver.CanComplete(task.TaskType))
        {
            yield return MinigameSolver.Complete(minigame, task);
            NeuroPlugin.Instance.Tasks.UpdatePathToTask(task);
            yield break;
        }

        yield return new WaitForSeconds(1);

        //task.Complete();
        if (task.TryCast<NormalPlayerTask>() is { } normalPlayerTask)
        {
            normalPlayerTask.NextStep();
            Info($"Task {normalPlayerTask} is at step {normalPlayerTask.TaskStep}/{normalPlayerTask.MaxStep}");

            // If NextStep() doesn't create an arrow, then this task does not require moving
            // to a different location and should be completed.
            if (normalPlayerTask.Arrow == null)
            {
                normalPlayerTask.Complete();
            }
        }
        else
        {
            Warning("Not Normal Player Task");
            task.Complete();
        }

        minigame.Close();

        NeuroPlugin.Instance.Tasks.UpdatePathToTask(task);
    }

    private static IEnumerator CompleteDoorMinigame(Minigame minigame)
    {
        // TODO: Refactor this
        if (minigame.TryCast<DoorBreakerGame>() is { } breakerGame)
        {
            yield return new WaitForSeconds(Random.RandomRange(1f, 2.5f));

            foreach (var button in breakerGame.Buttons)
            {
                if (button.flipX) breakerGame.FlipSwitch(button);
            }

            Info($"Opened breaker door {breakerGame.MyDoor.Id}");
        }
        else if (minigame.TryCast<DoorCardSwipeGame>() is { } swipeGame)
        {
            yield return new WaitForSeconds(1);

            ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, swipeGame.MyDoor.Id | 64);
            swipeGame.MyDoor.SetDoorway(true);
            swipeGame.StartCoroutine(swipeGame.CoStartClose(0.4f));

            Info($"Opened swipe door {swipeGame.MyDoor.Id}");
        }
        else
        {
            Warning($"{minigame.name} was not a {nameof(DoorBreakerGame)} or {nameof(DoorCardSwipeGame)}");
        }

        minigame.Close();
    }
}
