using HarmonyLib;
using UnityEngine;

namespace Neuro.Arrow.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        GameObject arrowObject = new("Arrow");
        NeuroPlugin.Instance.MainContext.ArrowHandler.Arrow = arrowObject.AddComponent<LineRenderer>();
    }
}
