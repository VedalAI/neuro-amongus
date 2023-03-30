using System;
using HarmonyLib;

namespace Neuro.Minigames.Completion;

[AttributeUsage(AttributeTargets.Class)]
public sealed class MinigameSolverAttribute : Attribute
{
    public MinigameSolverAttribute(TaskTypes firstTaskType, params TaskTypes[] otherTaskTypes)
    {
        TaskTypes = otherTaskTypes.AddToArray(firstTaskType);
    }

    public readonly TaskTypes[] TaskTypes;
}
