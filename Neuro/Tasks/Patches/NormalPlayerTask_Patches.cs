using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace Neuro.Tasks.Patches;

[HarmonyPatch(typeof(NormalPlayerTask), nameof(NormalPlayerTask.Initialize))]
public static class NormalPlayerTask_Initialize
{
    private static bool done = false;

    public static void Postfix(NormalPlayerTask __instance)
    {
        if (done) return;
        done = true;

        PlayerControl.LocalPlayer.StartCoroutine(NeuroPlugin.Instance.MainContext.TasksHandler.EvaluatePath(__instance));

        //NeuroPlugin.Instance.pathfinding.DrawPath(NeuroPlugin.Instance.currentPath))
    }
}
