using HarmonyLib;
using UnityEngine;

namespace Neuro.Cursor.Patches;

[HarmonyPatch(typeof(Input), nameof(Input.mousePosition), MethodType.Getter)]
public static class Input_get_mousePosition
{
    [HarmonyPostfix]
    public static void Postfix(ref Vector3 __result)
    {
        if (!ShipStatus.Instance) return;

        if (!InGameCursor.Instance.IsHidden)
        {
            __result = Camera.main!.WorldToScreenPoint(InGameCursor.Instance.Position);
        }
    }
}

[HarmonyPatch(typeof(Input), nameof(Input.GetMouseButton))]
public static class Input_GetMouseButton
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, int button)
    {
        if (!ShipStatus.Instance) return;

        if (button == 0 && InGameCursor.Instance.IsLeftButtonPressed)
        {
            __result = true;
        }
    }
}
