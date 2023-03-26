using HarmonyLib;
using Reactor.Utilities;
using BepInEx.Unity.IL2CPP.Utils;
using UnityEngine;

namespace Neuro.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
public static class Vent_EnterVent
{
    public static void Prefix(Vent __instance)
    {
        bool flag;
        bool flag2;
        __instance.CanUse(PlayerControl.LocalPlayer.Data, out flag, out flag2);
        if (!flag)
        {
            return;
        }

        PluginSingleton<NeuroPlugin>.Instance.didVent = true;
    }

    public static void Postfix(Vent __instance)
    {
        PlayerControl.LocalPlayer.StartCoroutine(PluginSingleton<NeuroPlugin>.Instance.Vent(__instance));
    }
}

[HarmonyPatch(typeof(Vent), nameof(Vent.ExitVent))]
public static class Vent_ExitVent
{
    public static void Prefix(Vent __instance)
    {
        bool flag;
        bool flag2;
        __instance.CanUse(PlayerControl.LocalPlayer.Data, out flag, out flag2);
        if (!flag)
        {
            return;
        }

        PluginSingleton<NeuroPlugin>.Instance.didVent = true;
    }
}
