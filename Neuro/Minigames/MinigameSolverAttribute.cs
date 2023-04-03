using System;
using HarmonyLib;

namespace Neuro.Minigames;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MinigameSolverAttribute : Attribute
{
    public MinigameSolverAttribute(Type firstType, params Type[] otherTypes)
    {
        Types = otherTypes.AddToArray(firstType);
    }

    public readonly Type[] Types;
}
