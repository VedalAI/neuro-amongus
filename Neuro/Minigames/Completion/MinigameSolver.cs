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
    // The larger this constant is, the longer the delays are when solving minigames.
    public const float DELAY_MULTIPLIER = 1;

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

        InGameCursor.Instance.HideWhen(() => !minigame);

        yield return new WaitForSeconds(0.4f);
        yield return solver.CompleteMinigame(minigame, task);
    }

    public abstract IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task);
}

public abstract class MinigameSolver<TMinigame> : MinigameSolver where TMinigame : Minigame
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask task)
        => CompleteMinigame(minigame.TryCast<TMinigame>(), task.TryCast<NormalPlayerTask>());

    public abstract IEnumerator CompleteMinigame(TMinigame minigame, NormalPlayerTask task);
}

public abstract class TasklessMinigameSolver<TMinigame> : MinigameSolver where TMinigame : Minigame
{
    public sealed override IEnumerator CompleteMinigame(Minigame minigame, PlayerTask _)
        => CompleteMinigame(minigame.TryCast<TMinigame>());

    public abstract IEnumerator CompleteMinigame(TMinigame minigame);
}
