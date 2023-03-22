using HarmonyLib;
using Reactor.Utilities;
using BepInEx.Unity.IL2CPP.Utils;

namespace Neuro.Patches;

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.Initialize))]
public static class NormalPlayerTask_Initialize
{
    static bool done = false;

    public static void Postfix(NormalPlayerTask __instance)
    {
        if (done) return;
        done = true;

        PlayerControl.LocalPlayer.StartCoroutine(PluginSingleton<NeuroPlugin>.Instance.EvaluatePath(__instance));

        //PluginSingleton<NeuroPlugin>.Instance.pathfinding.DrawPath(PluginSingleton<NeuroPlugin>.Instance.currentPath))
    }
}