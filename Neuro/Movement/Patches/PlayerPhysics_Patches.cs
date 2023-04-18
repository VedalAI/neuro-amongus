using HarmonyLib;
using UnityEngine;

namespace Neuro.Movement.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class PlayerPhysics_SetNormalizedVelocity
{
    [HarmonyPrefix]
    public static void Prefix(PlayerPhysics __instance, ref Vector2 direction)
    {
        if (!MovementHandler.Instance || !__instance.myPlayer.AmOwner) return;

        MovementHandler.Instance.GetForcedMoveDirection(ref direction);
    }
}
