using HarmonyLib;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(HauntMenuMinigame), nameof(HauntMenuMinigame.Begin))]
public static class HauntMenuMinigame_Begin
{
    [HarmonyPostfix]
    public static void Postfix(HauntMenuMinigame __instance)
    {
        __instance.SetFilter((int)HauntMenuMinigame.HauntFilters.Impostor);
    }
}