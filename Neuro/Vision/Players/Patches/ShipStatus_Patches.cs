using HarmonyLib;

namespace Neuro.Vision.Players.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake
{
    [HarmonyPostfix]
    public static void Postfix(ShipStatus __instance)
    {
        __instance.gameObject.AddComponent<PlayerControlVisionHandler>();
    }
}
