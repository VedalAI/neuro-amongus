using HarmonyLib;

namespace Neuro.Events.Patches;

[HarmonyPatch(typeof(GameManager), nameof(GameManager.StartGame))]
public static class GameManager_StartGame
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        EventManager.InvokeEvent(EventTypes.GameStarted);
    }
}
