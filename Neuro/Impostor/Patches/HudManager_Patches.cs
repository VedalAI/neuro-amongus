using HarmonyLib;

namespace Neuro.Impostor.Patches;

// Don't display "Task Complete" text when we are impostor
[HarmonyPatch(typeof(HudManager), nameof(HudManager.ShowTaskComplete))]
public static class HudManager_ShowTaskComplete
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return !PlayerControl.LocalPlayer.Data.Role.IsImpostor;
    }
}
