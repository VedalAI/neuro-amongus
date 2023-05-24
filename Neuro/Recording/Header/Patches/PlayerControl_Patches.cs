using HarmonyLib;

namespace Neuro.Recording.Header.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.SetRole))]
public static class PlayerControl_SetRole
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        Frame.ForceNextHeader = true;
    }
}
