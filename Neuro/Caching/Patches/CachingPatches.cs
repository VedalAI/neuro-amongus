using HarmonyLib;

namespace Neuro.Caching.Patches;

[HarmonyPatch]
public static class CachingPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    [HarmonyPostfix]
    public static void CacheDeadBodyPatch(DeathReason reason)
    {
        if (reason != DeathReason.Kill) return;
        ComponentCache<DeadBody>.FetchObjects();
    }
}
