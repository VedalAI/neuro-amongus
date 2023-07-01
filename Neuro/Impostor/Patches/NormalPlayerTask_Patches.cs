using HarmonyLib;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.AppendTaskText))]
[HarmonyPatch(typeof(AirshipUploadTask), nameof(AirshipUploadTask.AppendTaskText))]
[HarmonyPatch(typeof(TowelTask), nameof(TowelTask.AppendTaskText))]
[HarmonyPatch(typeof(UploadDataTask), nameof(UploadDataTask.AppendTaskText))]
[HarmonyPatch(typeof(WeatherNodeTask), nameof(WeatherNodeTask.AppendTaskText))]
public static class NormalPlayerTask_AppendTaskText
{
    [HarmonyPrefix]
    public static void Prefix(NormalPlayerTask __instance, ref (int step, NormalPlayerTask.TimerState timerState) __state)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

        __state.step = __instance.taskStep;
        __state.timerState = __instance.TimerStarted;

        __instance.taskStep = 0;
        __instance.TimerStarted = NormalPlayerTask.TimerState.NotStarted;
    }

    [HarmonyPostfix]
    public static void Postfix(NormalPlayerTask __instance, (int step, NormalPlayerTask.TimerState timerState) __state)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

        __instance.taskStep = __state.step;
        __instance.TimerStarted = __state.timerState;
    }
}

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.NextStep))]
public static class NormalPlayerTask_NextStep
{
    [HarmonyPrefix]
    public static void Prefix(NormalPlayerTask __instance, ref bool __state)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

        __state = __instance.ShowTaskStep;

        __instance.ShowTaskStep = false;
    }

    [HarmonyPostfix]
    public static void Postfix(NormalPlayerTask __instance, bool __state)
    {
        if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;

        __instance.ShowTaskStep = __state;
    }
}
