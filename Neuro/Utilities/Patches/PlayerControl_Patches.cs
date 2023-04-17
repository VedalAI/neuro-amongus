using HarmonyLib;

namespace Neuro.Utilities.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerControl_Die
{
    [HarmonyPostfix]
    public static void Postfix(DeathReason reason)
    {
        if (reason != DeathReason.Kill) return;
        ComponentCache<DeadBody>.FetchObjects();
    }
}
