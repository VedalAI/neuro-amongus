using HarmonyLib;
using UnityEngine;

namespace Neuro.Movement.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class PlayerPhysics_SetNormalizedVelocity
{
    [HarmonyPrefix]
    public static void Prefix(ref Vector2 direction)
    {
        MovementHandler.Instance.GetForcedMoveDirection(ref direction);
    }
}
