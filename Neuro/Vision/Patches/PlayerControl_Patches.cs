using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Vision.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
public static class PlayerControl_Start
{
    public static void Postfix(PlayerControl __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.MainContext.VisionHandler.StartTrackingPlayer(__instance);
    }
}
