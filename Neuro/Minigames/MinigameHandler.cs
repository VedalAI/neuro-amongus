using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames;

public static class MinigameHandler
{
    static MinigameHandler()
    {
        MinigameSolvers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<MinigameSolverAttribute>() is { })
            .Where(t => t.IsAssignableTo(typeof(MinigameSolver)))
            .Select(solverType => (solverType.GetCustomAttribute<MinigameSolverAttribute>()!.Types, solverType))
            .SelectMany(t => t.Types.Select(type => (type, Activator.CreateInstance(t.solverType))))
            .ToDictionary(t => Il2CppType.From(t.type).FullName, t => (MinigameSolver) t.Item2);
    }

    private static readonly Dictionary<string, MinigameSolver> MinigameSolvers;

    public static IEnumerator TryCompleteMinigame(Minigame minigame, PlayerTask task)
    {
        if (!MinigameSolvers.TryGetValue(minigame.GetIl2CppType().FullName, out MinigameSolver solver))
        {
            Warning($"Cannot solve minigame of type {minigame.GetIl2CppType().FullName}");
            yield break;
        }

        InGameCursor.Instance.HideWhen(() => !minigame);

        yield return new WaitForSeconds(0.4f);
        yield return solver.CompleteMinigame(minigame, task);

        // By this point we expect the solver to have completed the minigame,
        // which means that it will close and be destroyed, so this coroutine
        // will not execute any code below.
    }
}
