#if FULL
using HarmonyLib;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(TaskPanelBehaviour), nameof(TaskPanelBehaviour.Update))]
public static class TaskPanelBehaviour_Update
{
    [HarmonyPrefix]
    public static void Prefix(TaskPanelBehaviour __instance)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

        __instance.open = false;
    }
}
#endif
