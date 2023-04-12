using HarmonyLib;

namespace Neuro.Interactions.Patches;

[HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
public static class UseButton_SetTarget
{
    [HarmonyPrefix]
    public static void Postfix(UseButton __instance)
    {
        if (!InteractionsHandler.Instance || !__instance.canInteract) return;

        InteractionsHandler.Instance.UseTarget(__instance.currentTarget);
    }
}
