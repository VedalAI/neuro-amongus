using System.Collections;
using Neuro.Minigames.Completion;

namespace Neuro.Minigames;

public static class MinigameHandler
{
    public static IEnumerator TryCompleteMinigame(Minigame minigame, PlayerTask task)
    {
        if (MinigameSolver.CanComplete(minigame))
        {
            yield return MinigameSolver.Complete(minigame, task);
            NeuroPlugin.Instance.Tasks.UpdatePathToTask(task);
            yield break;
        }

        Warning($"Cannot solve minigame of type {minigame.GetIl2CppType().FullName}");


        /* If we don't know how to handle the minigame, we probably shouldn't touch it at all.

        // TODO: Everything below this should be removed once we have implemented all minigames and stuff
        yield return new WaitForSeconds(1);

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
        else if (task)
        {
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
        }
        else yield break;

        minigame.Close();

        */

        NeuroPlugin.Instance.Tasks.UpdatePathToTask(task);
    }
}
