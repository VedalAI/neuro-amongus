using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
public static class Minigame_Begin
{
    [HarmonyPostfix]
    public static void Postfix(Minigame __instance, PlayerTask task)
    {
        if (__instance.TryCast<VitalsMinigame>() || __instance.TryCast<SecurityLogGame>() || __instance.TryCast<TaskAdderGame>()
            || __instance.TryCast<PlanetSurveillanceMinigame>() || __instance.TryCast<SurveillanceMinigame>() || __instance.TryCast<EmergencyMinigame>()) return;

        // If this task is null, assume it is a door.
        __instance.StartCoroutine(task
            ? NeuroPlugin.Instance.Minigames.CompleteMinigame(task, __instance)
            : NeuroPlugin.Instance.Minigames.CompleteDoorMinigame(__instance));
    }
}
