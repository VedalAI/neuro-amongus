using HarmonyLib;
using UnityEngine;
using Neuro.Cursor;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Input), nameof(Input.mousePosition), MethodType.Getter)]
public static class Input_MousePosition_Patches
{
    [HarmonyPrefix]
    public static bool Prefix(ref Vector3 __result)
    {
        if (ShipStatus.Instance && !InGameCursor.Instance.IsHidden)
        {
            __result = Camera.main.WorldToScreenPoint(InGameCursor.Instance.Position);
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(Input), nameof(Input.GetMouseButton))]
public static class Input_GetMouseButton_Patches
{
    [HarmonyPrefix]
    public static bool Prefix(int button, ref bool __result)
    {
        if (button == 0 && ShipStatus.Instance && InGameCursor.Instance.IsMouseDown)
        {
            __result = true;
            return false;
        }
        return true;
    }
}