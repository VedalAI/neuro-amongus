using HarmonyLib;

namespace Neuro.Movement.Patches;

[HarmonyPatch(typeof(UseButton), nameof(UseButton.SetTarget))]
public class UseButton_SetTarget
{
    [HarmonyPrefix]
    public static void Prefix(IUsable target)
    {
        if (target is null) return;

        if (target.TryCast<DeconControl>() || target.TryCast<DoorConsole>())
        {
            // Automatically open Mira decontamination doors, automatically open locked doors
            // TODO: Intelligently integrate with auto movement
            target.Use();
        }
    }
}