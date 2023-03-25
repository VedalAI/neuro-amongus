using HarmonyLib;

namespace Neuro.Pathfinding.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.PathfindingHandler.Initialize();
    }
}
