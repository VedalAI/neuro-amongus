using HarmonyLib;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
public static class ExileController_WrapUp
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        EventHandler.InvokeEvent(EventTypes.MeetingEnded);
    }
}
