using HarmonyLib;
using Neuro.Vision.Players;

namespace Neuro.Vision.DeadBodies.Patches;

[HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.Awake))]
public static class ShipStatus_Awake
{
    [HarmonyPostfix]
    public static void Postfix(ShipStatus __instance)
    {
        __instance.gameObject.AddComponent<DeadBodyVisionHandler>();
    }
}
