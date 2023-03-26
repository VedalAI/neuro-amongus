using HarmonyLib;
using UnityEngine;

namespace Neuro.Movement.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        GameObject arrowObject = new("Arrow");
        NeuroPlugin.Instance.Movement.Arrow = arrowObject.AddComponent<LineRenderer>();
    }
}
