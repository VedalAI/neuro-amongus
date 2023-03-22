using HarmonyLib;
using Reactor.Utilities;
using UnityEngine;

namespace Neuro.Arrow.Patches;

// TODO: Do we need to also patch PolusShipStatus.Awake?
[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        GameObject arrowObject = new("Arrow");
        PluginSingleton<NeuroPlugin>.Instance.MainContext.ArrowHandler.Arrow = arrowObject.AddComponent<LineRenderer>();
    }
}
