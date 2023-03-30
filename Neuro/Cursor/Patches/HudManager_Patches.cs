using HarmonyLib;
using UnityEngine;

namespace Neuro.Cursor.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        // TODO: Load cursor asset or something
        GameObject cursor = new("InGameCursor");
        cursor.transform.SetParent(Camera.main!.transform, false);
        cursor.transform.localPosition = new Vector3(0f, 0f, -100f);
    }
}
