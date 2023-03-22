using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;

namespace Neuro.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.SetNormalizedVelocity))]
public static class PlayerPhysics_SetNormalizedVelocity
{
    public static bool Prefix(PlayerPhysics __instance, ref Vector2 direction)
    {
        return PluginSingleton<NeuroPlugin>.Instance.MovePlayer(ref direction);
    }
}