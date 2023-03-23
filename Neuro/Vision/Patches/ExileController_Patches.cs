using HarmonyLib;

namespace Neuro.Vision.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class ExileController_WrapUp
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.MainContext.VisionHandler.ResetAfterMeeting();
    }
}
