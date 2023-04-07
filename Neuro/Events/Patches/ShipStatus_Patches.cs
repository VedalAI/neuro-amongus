using HarmonyLib;
using Neuro.Vision.DeadBodies;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake
{
    [HarmonyPostfix]
    public static void Postfix(ShipStatus __instance)
    {
        EventHandler.InvokeEvent(EventTypes.GameStarted);
    }
}
