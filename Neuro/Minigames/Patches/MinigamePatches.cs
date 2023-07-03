using HarmonyLib;
using Neuro.Extensions.Harmony;

namespace Neuro.Minigames.Patches;

[FullHarmonyPatch]
public static class MinigamePatches
{
    [HarmonyPatch(typeof(Minigame), nameof(Minigame.Begin))]
    [HarmonyPostfix]
    public static void SolveMinigamePatch(Minigame __instance, PlayerTask task)
    {
        MinigameHandler.TryCompleteMinigame(__instance, task);
    }
}
