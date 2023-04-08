using HarmonyLib;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
public static class MeetingHud_Start
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        EventManager.InvokeEvent(EventTypes.MeetingStarted);
    }
}
