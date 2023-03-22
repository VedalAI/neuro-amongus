using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Vision.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class ExileController_WrapUp
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PluginSingleton<NeuroPlugin>.Instance.MainContext.VisionHandler.ResetAfterMeeting();
    }
}
