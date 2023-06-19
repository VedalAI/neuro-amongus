using HarmonyLib;
using System.Collections;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.AppendTaskText))]
[HarmonyPatch(typeof(AirshipUploadTask), nameof(AirshipUploadTask.AppendTaskText))]
[HarmonyPatch(typeof(TowelTask), nameof(TowelTask.AppendTaskText))]
[HarmonyPatch(typeof(UploadDataTask), nameof(UploadDataTask.AppendTaskText))]
[HarmonyPatch(typeof(WeatherNodeTask), nameof(WeatherNodeTask.AppendTaskText))]
public static class NormalPlayerTask_AppendTaskText
{
    // TODO: Certain tasks update StartAt, maybe we can collapse task list when we are impostor?

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
