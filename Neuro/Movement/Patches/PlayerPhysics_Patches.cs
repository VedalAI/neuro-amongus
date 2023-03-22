using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;

namespace Neuro.Movement.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class PlayerPhysics_SetNormalizedVelocity_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref Vector2 direction)
    {
        Vector2? newDirection = PluginSingleton<NeuroPlugin>.Instance.MainContext.MovementHandler.GetForcedMoveDirection(direction);
        if (newDirection.HasValue) direction = newDirection.Value;
    }
}
