using HarmonyLib;

namespace Neuro.Recording.LocalPlayer.Patches;

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageReactor))]
public static class MapRoom_SabotageReactor
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Reactor);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageComms))]
public static class MapRoom_SabotageComms
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Comms);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageLights))]
public static class MapRoom_SabotageLights
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Electrical);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageOxygen))]
public static class MapRoom_SabotageOxygen
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.LifeSupp);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageSeismic))]
public static class MapRoom_SabotageSeismic
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordSabotage(SystemTypes.Laboratory);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageDoors))]
public static class MapRoom_SabotageDoors
{
    [HarmonyPostfix]
    public static void Postfix(MapRoom __instance)
    {
        LocalPlayerRecorder.Instance.RecordDoors(__instance.room);
    }
}
