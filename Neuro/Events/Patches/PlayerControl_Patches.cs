using HarmonyLib;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerControl_Die
{
    [HarmonyPostfix]
    public static void Postfix(PlayerControl __instance, DeathReason reason)
    {
        EventManager.InvokeEvent(EventTypes.PlayerDied, __instance, reason);
    }
}
