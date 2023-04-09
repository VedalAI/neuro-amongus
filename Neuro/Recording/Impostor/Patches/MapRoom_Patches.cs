using HarmonyLib;

namespace Neuro.Recording.Impostor.Patches;

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageReactor))]
public class MapRoom_SabotageReactor
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        ImpostorRecorder.Instance.RecordSabotage(SystemTypes.Reactor);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageComms))]
public class MapRoom_SabotageComms
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        ImpostorRecorder.Instance.RecordSabotage(SystemTypes.Comms);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageLights))]
public class MapRoom_SabotageLights
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        ImpostorRecorder.Instance.RecordSabotage(SystemTypes.Electrical);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageOxygen))]
public class MapRoom_SabotageOxygen
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        ImpostorRecorder.Instance.RecordSabotage(SystemTypes.LifeSupp);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageSeismic))]
public class MapRoom_SabotageSeismic
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        ImpostorRecorder.Instance.RecordSabotage(SystemTypes.Laboratory);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageDoors))]
public class MapRoom_SabotageDoors
{
    [HarmonyPostfix]
    public static void Postfix(MapRoom __instance)
    {
        ImpostorRecorder.Instance.RecordDoors(__instance.room);
    }
}
