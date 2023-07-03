using HarmonyLib;
using Neuro.Extensions.Harmony;

namespace Neuro.Debugging.Patches;

[HarmonyPatch]
public static class GameTestingPatches
{
    [HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
    [HarmonyPostfix]
    public static void NeverBannedPatch(out bool __result)
    {
        __result = false;
    }

    [DebugHarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    [DebugHarmonyPrefix]
    public static void StartWithOnePlayerPatch(GameStartManager __instance)
    {
        __instance.MinPlayers = 1;
    }
}