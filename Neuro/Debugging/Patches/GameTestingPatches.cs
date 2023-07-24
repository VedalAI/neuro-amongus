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
        // TODO: why is this not working
        __instance.MinPlayers = 1;
    }

    [DebugHarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
    [DebugHarmonyPatch(typeof(LogicGameFlowHnS), nameof(LogicGameFlowHnS.CheckEndCriteria))]
    [DebugHarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.IsGameOverDueToDeath))]
    [DebugHarmonyPatch(typeof(LogicGameFlowHnS), nameof(LogicGameFlowHnS.IsGameOverDueToDeath))]
    [DebugHarmonyPrefix]
    [HarmonyPriority(Priority.First)]
    public static bool StopGameEndingPatch() => false;
}
