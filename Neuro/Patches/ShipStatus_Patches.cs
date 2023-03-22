using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.FixedUpdate))]
public static class ShipStatus_FixedUpdate
{
    public static void Postfix(ShipStatus __instance)
    {
        if (!PluginSingleton<NeuroPlugin>.Instance.hasStarted)
        {
            PluginSingleton<NeuroPlugin>.Instance.hasStarted = true;

            PluginSingleton<NeuroPlugin>.Instance.StartMap(__instance);
        }
    }
}

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.OnDestroy))]
public static class ShipStatus_OnDestroy
{
    public static void Postfix(ShipStatus __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.hasStarted = false;
    }
}