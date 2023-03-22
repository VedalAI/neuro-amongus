using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Vision.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class ExileController_WrapUp
{
    public static void Postfix(ExileController __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.MainContext.VisionHandler.ResetAfterMeeting();
    }
}
