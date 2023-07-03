using HarmonyLib;
using Neuro.Caching;
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
            __result = UnityCache.MainCamera.WorldToScreenPoint(InGameCursor.Instance.Position);
        }
    }
}

[HarmonyPatch(typeof(Input), nameof(Input.GetMouseButton))]
public static class Input_GetMouseButton
{
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, int button)
    {
        if (!ShipStatus.Instance || button != 0) return;

        if (!InGameCursor.Instance.IsHidden)
        {
            __result = InGameCursor.Instance.IsLeftButtonPressed;
        }
    }
}