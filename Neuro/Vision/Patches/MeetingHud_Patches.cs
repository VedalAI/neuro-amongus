using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Vision.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class MeetingHud_Start_Patch
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PluginSingleton<NeuroPlugin>.Instance.MainContext.VisionHandler.ReportFindings();
    }
}
