using HarmonyLib;
using Neuro.Extensions.Harmony;
using UnityEngine;

namespace Neuro.Movement.Patches;

[FullHarmonyPatch]
public static class MovementPatches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
    [HarmonyPrefix]
    public static void OverrideVelocityPatch(PlayerPhysics __instance, ref Vector2 direction)
    {
        if (!MovementHandler.Instance || !__instance.myPlayer.AmOwner) return;

        MovementHandler.Instance.GetForcedMoveDirection(ref direction);
    }
}
