using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Vision.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class MeetingHud_Start_Patch
{
    public static void Postfix(EmergencyMinigame __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.VisionHandler.ReportFindings();
    }
}
