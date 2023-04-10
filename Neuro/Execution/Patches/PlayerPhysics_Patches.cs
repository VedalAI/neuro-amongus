using HarmonyLib;
using UnityEngine;

namespace Neuro.Execution.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class PlayerPhysics_SetNormalizedVelocity
{
    [HarmonyPrefix]
    public static void Prefix(ref Vector2 direction)
    {
        if (NeuroPlugin.Instance.AIEnabled)
        {
            direction = NeuroPlugin.Instance.Executor.movementDirection;
        }
    }
}
