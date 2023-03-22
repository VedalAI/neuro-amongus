using HarmonyLib;
using Reactor.Utilities;

namespace Neuro.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
public static class PlayerControl_Start
{
    public static void Postfix(PlayerControl __instance)
    {
        PluginSingleton<NeuroPlugin>.Instance.vision.UpdatePlayerControlArray(__instance);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
public static class PlayerControlFixedUpdate
{
    public static void Postfix(PlayerControl __instance)
    {
        if (__instance.AmOwner) PluginSingleton<NeuroPlugin>.Instance.FixedUpdate();
    }
}
