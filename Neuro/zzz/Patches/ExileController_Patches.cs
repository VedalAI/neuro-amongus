using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class ExileController_WrapUp
{
    public static void Postfix(ExileController __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.MeetingEnd();
    }
}