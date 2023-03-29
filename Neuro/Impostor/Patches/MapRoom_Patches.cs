using HarmonyLib;
using Neuro.Recording.DataStructures;
using System.Linq;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageReactor))]
public class MapRoom_SabotageReactor
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.Recording.RecordSabotage(SabotageTypes.Reactor);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageComms))]
public class MapRoom_SabotageComms
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.Recording.RecordSabotage(SabotageTypes.Comms);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageLights))]
public class MapRoom_SabotageLights
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.Recording.RecordSabotage(SabotageTypes.Lights);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageOxygen))]
public class MapRoom_SabotageOxygen
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.Recording.RecordSabotage(SabotageTypes.Oxygen);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageSeismic))]
public class MapRoom_SabotageSeismic
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.Recording.RecordSabotage(SabotageTypes.Seismic);
    }
}

[HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageDoors))]
public class MapRoom_SabotageDoors
{
    [HarmonyPostfix]
    public static void Postfix(MapRoom __instance)
    {
        NeuroPlugin.Instance.Recording.RecordDoors(__instance.room);
    }
}

