extern alias JetBrains;
using HarmonyLib;
using Neuro.Extensions.Harmony;
using UsedImplicitly = JetBrains::JetBrains.Annotations.UsedImplicitlyAttribute;

namespace Neuro.Impostor.Patches;

[FullHarmonyPatch]
public static class TaskFakingPatches
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.ShowTaskComplete))]
    [HarmonyPrefix]
    public static bool DontShowTaskCompletePatch()
    {
        return !PlayerControl.LocalPlayer.Data.Role.IsImpostor;
    }

    [HarmonyPatch(typeof(TaskPanelBehaviour), nameof(TaskPanelBehaviour.Update))]
    [HarmonyPrefix]
    public static void CloseTaskListPatch(TaskPanelBehaviour __instance)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

        __instance.open = false;
    }

    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.UpdateArrow))]
    [HarmonyPrefix]
    public static bool HideTaskArrowPatch()
    {
        return !PlayerControl.LocalPlayer.Data.Role.IsImpostor;
    }

    [HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.NextStep))]
    public static class DontPlayTaskUpdateSoundPatch
    {
        [HarmonyPrefix]
        [UsedImplicitly]
        public static void Prefix(NormalPlayerTask __instance, ref bool __state)
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

            __state = __instance.ShowTaskStep;

            __instance.ShowTaskStep = false;
        }

        [HarmonyPostfix]
        [UsedImplicitly]
        public static void Postfix(NormalPlayerTask __instance, bool __state)
        {
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

            __instance.ShowTaskStep = __state;
        }
    }
}
