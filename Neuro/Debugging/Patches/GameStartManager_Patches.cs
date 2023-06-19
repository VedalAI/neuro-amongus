#if FULL
using HarmonyLib;

namespace Neuro.Debugging.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
public static class GameStartManager_Update_Patch
{
    [HarmonyPrefix]
    public static void Prefix(GameStartManager __instance)
    {
        __instance.MinPlayers = 1;
    }
}
#endif
