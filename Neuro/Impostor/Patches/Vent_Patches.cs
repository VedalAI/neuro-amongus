using HarmonyLib;
using BepInEx.Unity.IL2CPP.Utils;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(Vent), nameof(Vent.EnterVent))]
public static class Vent_EnterVent
{
    public static void Postfix(Vent __instance)
    {
        PlayerControl.LocalPlayer.StartCoroutine(NeuroPlugin.Instance.Impostor.Vent(__instance));
    }
}
