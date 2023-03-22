using System.Collections;
using Neuro.DependencyInjection;
using Neuro.Utilities;
using Reactor.Utilities;
using UnityEngine;

namespace Neuro.Minigames;

public class MinigamesHandler : IMinigamesHandler
{
    public IContextProvider Context { get; set; }

    public IEnumerator CompleteMinigame(PlayerTask task, Minigame minigame)
    {
        yield return new WaitForSeconds(2);

        //task.Complete();
        if (task.TryCast<NormalPlayerTask>() is { } normalPlayerTask)
        {
            normalPlayerTask.NextStep();
        }
        else
        {
            Debug.Log("Not Normal Player Task");
            task.Complete();
        }

        minigame.Close();

        GetPathToNextTask(task);
    }

    // TODO: This should be somewhere else
    public void GetPathToNextTask(PlayerTask lastTask)
    {
        PlayerTask nextTask = null;
        if (lastTask.IsComplete)
        {
            Debug.Log("Task is complete");
            foreach (PlayerTask t in PlayerControl.LocalPlayer.myTasks)
            {
                if (!t.IsComplete && t.HasLocation)
                {
                    nextTask = t;
                    Debug.Log(nextTask.name);
                    break;
                }
            }
        }
        else
        {
            nextTask = lastTask;
        }

        if (nextTask != null)
        {
            Debug.Log("Next task isn't null");
            PluginSingleton<NeuroPlugin>.Instance.MainContext.TasksHandler.CurrentPath = PluginSingleton<NeuroPlugin>.Instance.MainContext.PathfindingHandler.FindPath(PlayerControl.LocalPlayer.transform.position, nextTask.Locations.At(0));
            PluginSingleton<NeuroPlugin>.Instance.MainContext.TasksHandler.PathIndex = 0;
        }
    }
}
