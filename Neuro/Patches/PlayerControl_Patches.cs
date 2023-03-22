using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;

namespace Neuro.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
public static class PlayerControl_Start
{
    public static void Postfix(PlayerControl __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.vision.UpdatePlayerControlArray(__instance);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControlFixedUpdate
{
    public static void Postfix(PlayerControl __instance)
    {
        if (PlayerControl.LocalPlayer == __instance) PluginSingleton<NeuroPlugin>.Instance.FixedUpdate(__instance);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerControl_Die
{
    public static void Postfix(DeathReason reason, PlayerControl __instance)
    {
        // Check if player was murdered
        if (reason == DeathReason.Kill)
        {
            // Find dead body at death position
            Collider2D[] deathPointColliders = Physics2D.OverlapCircleAll(__instance.GetTruePosition(), 1f);
            foreach (Collider2D col in deathPointColliders)
            {
                // Call vision method for handling new dead bodies
                DeadBody deadBody = col.gameObject.GetComponent<DeadBody>();
                if (deadBody)
                {
                    PluginSingleton<NeuroPlugin>.Instance.vision.DeadBodyAppeared(deadBody);
                }
            }
        }
    }
}