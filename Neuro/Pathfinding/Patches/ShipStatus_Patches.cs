using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Pathfinding.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PluginSingleton<NeuroPlugin>.Instance.MainContext.PathfindingHandler.Initialize();
    }
}
