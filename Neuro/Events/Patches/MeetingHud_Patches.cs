using HarmonyLib;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class MeetingHud_Start
{
    [HarmonyPostfix]
    public static void Postfix(MeetingHud __instance)
    {
        EventManager.InvokeEvent(EventTypes.MeetingStarted, __instance);
    }
}
