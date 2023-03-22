using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
public static class Minigame_Begin_Patch
{
    [HarmonyPostfix]
    public static void Postfix(Minigame __instance, PlayerTask task)
    {
        __instance.StartCoroutine(PluginSingleton<NeuroPlugin>.Instance.MainContext.MinigamesHandler.CompleteMinigame(task, __instance));
    }
}
