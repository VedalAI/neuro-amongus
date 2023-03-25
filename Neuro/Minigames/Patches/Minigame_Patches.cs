using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
public static class Minigame_Begin
{
    [HarmonyPostfix]
    public static void Postfix(Minigame __instance, PlayerTask task)
    {
        __instance.StartCoroutine(NeuroPlugin.Instance.MinigamesHandler.CompleteMinigame(task, __instance));
    }
}
