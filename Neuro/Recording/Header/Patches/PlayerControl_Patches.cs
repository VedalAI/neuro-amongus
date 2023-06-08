using HarmonyLib;

namespace Neuro.Recording.Header.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
public static class PlayerControl_SetRole
{
    [HarmonyPostfix]
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.AmOwner) Frame.ForceNextHeader = true;
    }
}
