using HarmonyLib;

namespace Neuro.Vision.Players.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class ExileController_WrapUp
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PlayerControlVisionHandler.Instance.ResetAfterMeeting();
    }
}
