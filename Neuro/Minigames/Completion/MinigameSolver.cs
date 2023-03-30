using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Neuro.Minigames.Completion;

public abstract class MinigameSolver
{
    static MinigameSolver()
    {
        MinigameSolvers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<MinigameSolverAttribute>() is { })
            .Where(t => t.IsAssignableTo(typeof(MinigameSolver)))
            .Select(solverType => (solverType.GetCustomAttribute<MinigameSolverAttribute>()!.TaskTypes, solverType))
            .SelectMany(t => t.TaskTypes.Select(taskType => (taskType, t.solverType)))
            .ToDictionary(t => t.taskType, t => t.solverType);
    }

    private static readonly Dictionary<TaskTypes, Type> MinigameSolvers;

    public static bool CanComplete(TaskTypes taskType) => MinigameSolvers.ContainsKey(taskType);
    public static IEnumerator Complete(Minigame minigame, NormalPlayerTask task)
    {
        if (!CanComplete(task.TaskType)) throw new ArgumentException($"Cannot solve task type {task.TaskType}");

        MinigameSolver solver = (MinigameSolver) Activator.CreateInstance(MinigameSolvers[task.TaskType])!;
        return solver.CompleteMinigame(minigame, task);
    }

    public abstract IEnumerator CompleteMinigame(Minigame minigame, NormalPlayerTask task);
}

public abstract class MinigameSolver<T> : MinigameSolver where T : Minigame
{
    private T _myMinigame;

    protected T MyMinigame => _myMinigame ? _myMinigame : throw new SolveInterruptedException();
    protected NormalPlayerTask MyTask { get; private set; }

    protected abstract IEnumerator CompleteMinigame();

    public override IEnumerator CompleteMinigame(Minigame minigame, NormalPlayerTask task)
    {
        _myMinigame = minigame.Cast<T>();
        MyTask = task;

        IEnumerator enumerator = CompleteMinigame();

        yield return new WaitForSeconds(0.4f);

        while (true)
        {
            object current;
            try
            {
                if (enumerator.MoveNext() == false)
                {
                    break;
                }
                current = enumerator.Current;
            }
            catch (SolveInterruptedException)
            {
                yield break;
            }
            yield return current;
        }

        while (_myMinigame) yield return null;
    }
}
