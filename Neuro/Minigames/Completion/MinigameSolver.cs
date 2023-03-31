using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion;

public abstract class MinigameSolver
{
    static MinigameSolver()
    {
        MinigameSolvers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<MinigameSolverAttribute>() is { })
            .Where(t => t.IsAssignableTo(typeof(MinigameSolver)))
            .Select(solverType => (solverType.GetCustomAttribute<MinigameSolverAttribute>()!.Types, solverType))
            .SelectMany(t => t.Types.Select(type => (type, Activator.CreateInstance(t.solverType))))
            .ToDictionary(t => Il2CppType.From(t.type).FullName, t => (MinigameSolver) t.Item2);
    }

    private static readonly Dictionary<string, MinigameSolver> MinigameSolvers;

    public static bool CanComplete(Minigame minigame) => MinigameSolvers.ContainsKey(minigame.GetIl2CppType().FullName);
    public static IEnumerator Complete(Minigame minigame, PlayerTask task)
    {
        if (!CanComplete(minigame)) throw new ArgumentException($"Cannot solve minigame {minigame.name}");

        MinigameSolver solver = MinigameSolvers[minigame.GetIl2CppType().FullName];

        yield return new WaitForSeconds(0.4f);
        yield return solver.CompleteMinigame(minigame, task);

        InGameCursor.Instance.HideWhen(() => !minigame);
    }

    public abstract IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task);
}

public abstract class MinigameSolver<TMinigame, TTask> : MinigameSolver where TMinigame : Minigame where TTask : PlayerTask
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task)
        => CompleteMinigame(minigame.TryCast<TMinigame>(), task.TryCast<TTask>());

    public abstract IEnumerator CompleteMinigame(TMinigame minigame, TTask task);
}

public abstract class MinigameSolver<TMinigame> : MinigameSolver<TMinigame, NormalPlayerTask> where TMinigame : Minigame
{
}
