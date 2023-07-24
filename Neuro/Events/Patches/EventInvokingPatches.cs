using HarmonyLib;

namespace Neuro.Events.Patches;

[HarmonyPatch]
public static class EventInvokingPatches
{
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    [HarmonyPostfix]
    public static void ExileCutsceneEndedPatch()
    {
        EventManager.InvokeEvent(EventTypes.ExileCutsceneEnded);
    }

    [HarmonyPatch(typeof(GameManager), nameof(GameManager.StartGame))]
    [HarmonyPostfix]
    public static void GameStartedPatch()
    {
        EventManager.InvokeEvent(EventTypes.GameStarted);
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    [HarmonyPostfix]
    public static void MeetingStartedPatch(MeetingHud __instance)
    {
        EventManager.InvokeEvent(EventTypes.MeetingStarted, __instance);
    }

    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Close))]
    [HarmonyPostfix]
    public static void MeetingEndedPatch()
    {
        EventManager.InvokeEvent(EventTypes.MeetingEnded);
    }
}
