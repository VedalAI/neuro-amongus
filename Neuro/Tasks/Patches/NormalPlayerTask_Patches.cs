using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Tasks.Patches;

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.Initialize))]
public static class NormalPlayerTask_Initialize
{
    private static bool done = false;

    public static void Postfix(NormalPlayerTask __instance)
    {
        if (done) return;
        done = true;

        PlayerControl.LocalPlayer.StartCoroutine(PluginSingleton<NeuroPlugin>.Instance.MainContext.TasksHandler.EvaluatePath(__instance));

        //PluginSingleton<NeuroPlugin>.Instance.pathfinding.DrawPath(PluginSingleton<NeuroPlugin>.Instance.currentPath))
    }
}
