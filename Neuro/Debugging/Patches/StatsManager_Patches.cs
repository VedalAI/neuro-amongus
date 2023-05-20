using HarmonyLib;

namespace Neuro.Debugging.Patches;

#if FULL
[HarmonyPatch(typeof(StatsManager), nameof(StatsManager.AmBanned), MethodType.Getter)]
public static class StatsManager_get_AmBanned_Patch
{
    [HarmonyPostfix]
    public static void Postfix(out bool __result)
    {
        __result = false;
    }
}
#endif
