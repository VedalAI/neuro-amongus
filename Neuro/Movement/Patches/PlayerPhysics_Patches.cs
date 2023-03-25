using HarmonyLib;
using UnityEngine;

namespace Neuro.Movement.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class PlayerPhysics_SetNormalizedVelocity_Patch
{
    [HarmonyPrefix]
    public static void Prefix(ref Vector2 direction)
    {
        NeuroPlugin.Instance.MovementHandler.GetForcedMoveDirection(ref direction);
    }
}
