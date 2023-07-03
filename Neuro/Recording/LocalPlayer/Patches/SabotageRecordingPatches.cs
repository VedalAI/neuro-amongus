using HarmonyLib;

namespace Neuro.Recording.LocalPlayer.Patches;

[HarmonyPatch]
public static class SabotageRecordingPatches
{
    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageReactor))]
    [HarmonyPostfix]
    public static void RecordReactorPatch()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Reactor);
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageComms))]
    [HarmonyPostfix]
    public static void RecordCommsPatch()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Comms);
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageLights))]
    [HarmonyPostfix]
    public static void RecordLightsPatch()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Electrical);
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageOxygen))]
    [HarmonyPostfix]
    public static void RecordO2Patch()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.LifeSupp);
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageSeismic))]
    [HarmonyPostfix]
    public static void RecordSeismicPatch()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Laboratory);
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageDoors))]
    [HarmonyPostfix]
    public static void RecordDoorsPatch(MapRoom __instance)
    {
        LocalPlayerRecorder.Instance.RecordDoors(__instance.room);
    }
}
