using System.Diagnostics;
using HarmonyLib;

namespace Neuro.Debugging.Patches;

#if FULL
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
