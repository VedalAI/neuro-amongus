using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;

namespace Neuro.Minigames;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MinigameSolverAttribute : Attribute
{
    static MinigameSolverAttribute()
    {
        MinigameSolvers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<MinigameSolverAttribute>() is { })
            .Where(t => t.IsAssignableTo(typeof(MinigameSolver)))
            .Select(solverType => (solverType.GetCustomAttribute<MinigameSolverAttribute>()!.Types, solverType))
            .SelectMany(t => t.Types.Select(type => (type, Activator.CreateInstance(t.solverType))))
            .ToDictionary(t => Il2CppType.From(t.type).FullName, t => (MinigameSolver) t.Item2);
    }

    public static Dictionary<string, MinigameSolver> MinigameSolvers { get; }

    public MinigameSolverAttribute(Type firstType, params Type[] otherTypes)
    {
        Types = otherTypes.AddToArray(firstType);
    }

    public readonly Type[] Types;
}
