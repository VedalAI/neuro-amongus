using HarmonyLib;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake
{
    [HarmonyPostfix]
    public static void Postfix(ShipStatus __instance)
    {
        EventManager.InvokeEvent(EventTypes.GameStarted, __instance);
    }
}
