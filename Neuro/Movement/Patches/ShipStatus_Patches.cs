using HarmonyLib;
using UnityEngine;

namespace Neuro.Movement.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        GameObject arrowObject = new("Arrow");
        NeuroPlugin.Instance.MainContext.MovementHandler.Arrow = arrowObject.AddComponent<LineRenderer>();
    }
}
