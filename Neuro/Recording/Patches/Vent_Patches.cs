using HarmonyLib;

namespace Neuro.Recording.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
public static class Vent_Use
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.RecordingHandler.DidVent = true;
    }
}
