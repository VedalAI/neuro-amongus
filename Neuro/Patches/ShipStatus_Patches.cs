using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake
{
    public static void Postfix(ShipStatus __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.StartMap(__instance);
    }
}
