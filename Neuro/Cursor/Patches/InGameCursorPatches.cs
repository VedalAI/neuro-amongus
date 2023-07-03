using HarmonyLib;
using Neuro.Extensions.Harmony;
using UnityEngine;

namespace Neuro.Cursor.Patches;

[FullHarmonyPatch]
public static class InGameCursorPatches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    [HarmonyPostfix]
    public static void CreateCursorPatch()
    {
        new GameObject("InGameCursor").AddComponent<InGameCursor>();
    }
}
