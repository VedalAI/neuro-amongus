using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Patches;

[HarmonyPatch(typeof(EmergencyMinigame), nameof(EmergencyMinigame.CallMeeting))]
public static class EmergencyMinigame_CallMeeting
{
    public static void Postfix(EmergencyMinigame __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.MeetingBegin();
    }
}