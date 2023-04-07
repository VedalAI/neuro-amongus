using HarmonyLib;
using Neuro.Vision.DeadBodies;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        EventManager.InvokeEvent(EventTypes.GameStarted);
    }
}
