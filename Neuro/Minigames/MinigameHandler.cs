using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames;

public static class MinigameHandler
{
    [Conditional("FULL")]
    public static void TryCompleteMinigame(Minigame minigame, PlayerTask task)
    {
        GameObject coroutineObject = new("Minigame Solver");
        coroutineObject.transform.parent = minigame.transform;

        // DivertPowerMetagame doesn't run any logic by itself
        MonoBehaviour coroutineParent = coroutineObject.AddComponent<DivertPowerMetagame>();
        coroutineParent.StartCoroutine(CoTryCompleteMinigame(minigame, task, coroutineParent));
    }

    public static bool ShouldOpenConsole(Console console, Minigame minigame, PlayerTask task)
    {
        if (!MinigameOpenerAttribute.MinigameOpeners.TryGetValue(minigame.GetIl2CppType().FullName, out List<IMinigameOpener> openers))
            return false;

        return openers.Any(o => o.ShouldOpenConsole(console, task));
    }

    private static IEnumerator CoTryCompleteMinigame(Minigame minigame, PlayerTask task, MonoBehaviour coroutineParent)
    {
        if (!MinigameSolverAttribute.MinigameSolvers.TryGetValue(minigame.GetIl2CppType().FullName, out IMinigameSolver solver))
        {
            Warning($"Cannot solve minigame of type {minigame.GetIl2CppType().FullName}");
            yield break;
        }

        InGameCursor.Instance.HideWhen(() => !minigame);
        MinigameTimeHandler.Instance.StartTimer(minigame, () => !minigame);

        coroutineParent.StartCoroutine(CloseMinigameWithDelay(minigame, solver.CloseTimout));

        yield return new WaitForSeconds(0.4f);
        yield return solver.CompleteMinigame(minigame, task);

        // By this point we expect the solver to have completed the minigame,
        // which means that it will close and be destroyed, so this coroutine
        // will not execute any code below.
    }

    private static IEnumerator CloseMinigameWithDelay(Minigame minigame, float delay)
    {
        yield return new WaitForSeconds(delay);
        minigame.Close();
    }
}
