using HarmonyLib;

namespace Neuro.Vision.DeadBodies.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class ExileController_WrapUp
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        DeadBodyVisionHandler.Instance.ResetAfterMeeting();
    }
}
