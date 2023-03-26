using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.Use))]
public static class Vent_Use
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
