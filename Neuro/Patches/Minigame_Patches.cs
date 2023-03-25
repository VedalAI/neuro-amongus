using HarmonyLib;
using Reactor.Utilities;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;
using Neuro.Utils;
using Il2CppSystem;

namespace Neuro.Patches;

[HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
public static class Minigame_Begin
{
    public static void Postfix(Minigame __instance, PlayerTask task)
    {
        Debug.Log("Started task + " + __instance.name);

        (float min, float max) timeToComplete = Methods.TaskTypeToTimeToCompleteTask(__instance.TaskType);
        float timeToWait = UnityEngine.Random.RandomRange(timeToComplete.min, timeToComplete.max);

        __instance.StartCoroutine(MinigameAutocomplete(__instance, task, timeToWait));
    }

    public static IEnumerator MinigameAutocomplete(Minigame minigame, PlayerTask task, float time)
    {
        yield return new WaitForSeconds(time);

        if (task.TryCast<NormalPlayerTask>() is NormalPlayerTask normalPlayerTask)
        {
            normalPlayerTask.NextStep();
            Debug.Log(String.Format("Task {0} is at step {1}/{2}", normalPlayerTask, normalPlayerTask.TaskStep, normalPlayerTask.MaxStep));

            // If NextStep() doesn't create an arrow, then this task does not require moving
            // to a different location and should be completed.
            if (normalPlayerTask.Arrow == null) {
                normalPlayerTask.Complete();
            }
        }
        else
        {
            Debug.Log("Not Normal Player Task");
            task.Complete();
        }
        minigame.Close();
        PluginSingleton<NeuroPlugin>.Instance.inMinigame = false;

        PluginSingleton<NeuroPlugin>.Instance.UpdatePathToTask(task);
    }
}