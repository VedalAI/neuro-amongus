using HarmonyLib;

namespace Neuro.Recording.Header.Patches;

[HarmonyPatch]
public static class ResendHeaderPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
    [HarmonyPostfix]
    public static void ResendHeaderOnRoleChangePatch(PlayerControl __instance)
    {
        if (__instance.AmOwner) Frame.ForceNextHeader = true;
    }
}