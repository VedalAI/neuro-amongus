using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;

namespace Neuro.Minigames;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class MinigameSolverAttribute : MinigameOpenerAttribute
{
    static MinigameSolverAttribute()
    {
        MinigameSolvers = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .Where(type => type.IsAssignableTo(typeof(IMinigameSolver)))
            .SelectMany(solver => solver.GetCustomAttributes<MinigameSolverAttribute>()
                .Select(attribute => (solver, minigame: attribute.Type)))
            .Where(item => item.minigame is not null)
            .ToDictionary(item => Il2CppType.From(item.minigame).FullName,
                item => (IMinigameSolver) Activator.CreateInstance(item.solver));
    }

    public static Dictionary<string, IMinigameSolver> MinigameSolvers { get; }

    public MinigameSolverAttribute(Type type, bool opens = true) : base(opens ? type : null)
    {
        Type = type;
    }

    public new readonly Type Type;
}
