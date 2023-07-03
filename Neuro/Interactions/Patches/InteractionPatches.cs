using HarmonyLib;
using Neuro.Extensions.Harmony;

namespace Neuro.Interactions.Patches;

[FullHarmonyPatch]
public static class InteractionPatches
{
    [HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
    [HarmonyPrefix]
    public static void UseInteractablePatch(UseButton __instance)
    {
        if (!__instance.canInteract) return;

        InteractionsHandler.UseTarget(__instance.currentTarget);
    }
}
