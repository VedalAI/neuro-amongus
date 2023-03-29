using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
public static class Minigame_Begin
{
    [HarmonyPostfix]
    public static void Postfix(Minigame __instance, PlayerTask task)
    {
        // ignore the practice mode laptop, which is apparently considered a minigame but has no TaskType
        if (__instance.TryCast<TaskAdderGame>()) return;
        __instance.StartCoroutine(NeuroPlugin.Instance.Minigames.CompleteMinigame(task, __instance));
    }
}
