using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
public static class Minigame_Begin
{
    [HarmonyPostfix]
    public static void Postfix(Minigame __instance, PlayerTask task)
    {
        // If this task is null, assume it is a door. TODO: Are there other instances where the task is null?
        __instance.StartCoroutine(task
            ? NeuroPlugin.Instance.Minigames.CompleteMinigame(task, __instance)
            : NeuroPlugin.Instance.Minigames.CompleteDoorMinigame(__instance));
    }
}
