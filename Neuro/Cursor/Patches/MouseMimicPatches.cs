using HarmonyLib;
using Neuro.Caching;
using Neuro.Extensions.Harmony;
using UnityEngine;

namespace Neuro.Cursor.Patches;

[FullHarmonyPatch]
public static class MouseMimicPatches
{
    [HarmonyPatch(typeof(Input), nameof(Input.mousePosition), MethodType.Getter)]
    [HarmonyPostfix]
    public static void Postfix(ref Vector3 __result)
    {
        if (!ShipStatus.Instance) return;

        if (!InGameCursor.Instance.IsHidden)
        {
            __result = UnityCache.MainCamera.WorldToScreenPoint(InGameCursor.Instance.Position);
        }
    }

    [HarmonyPatch(typeof(Input), nameof(Input.GetMouseButton))]
    [HarmonyPostfix]
    public static void Postfix(ref bool __result, int button)
    {
        if (!ShipStatus.Instance || button != 0) return;

        if (!InGameCursor.Instance.IsHidden)
        {
            __result = InGameCursor.Instance.IsLeftButtonPressed;
        }
    }

    [HarmonyPatch(typeof(Application), nameof(Application.isFocused), MethodType.Getter)]
    [HarmonyPrefix]
    public static bool AlwaysFocusedPatch(out bool __result)
    {
        __result = true;
        return false;
    }
}
