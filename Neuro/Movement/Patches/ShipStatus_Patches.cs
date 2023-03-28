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
        LineRenderer arrow = NeuroPlugin.Instance.Movement.Arrow = arrowObject.AddComponent<LineRenderer>();

        arrow.startWidth = 0.4f;
        arrow.endWidth = 0.05f;
        arrow.positionCount = 2;
        arrow.material = new Material(Shader.Find("Sprites/Default"));
        arrow.startColor = Color.blue;
        arrow.endColor = Color.cyan;
    }
}
