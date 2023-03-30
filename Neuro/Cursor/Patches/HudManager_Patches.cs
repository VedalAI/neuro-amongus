using HarmonyLib;
using UnityEngine;

namespace Neuro.Cursor.Patches;

[HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
public static class HudManager_Start
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        new GameObject("InGameCursor").AddComponent<InGameCursor>();
    }
}
