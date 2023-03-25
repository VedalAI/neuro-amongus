using System.Linq;
using HarmonyLib;
using UnityEngine;

namespace Neuro.Vision.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Start))]
public static class PlayerControl_Start
{
    public static void Postfix(PlayerControl __instance)
    {
        NeuroPlugin.Instance.VisionHandler.StartTrackingPlayer(__instance);
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
public static class PlayerControl_Die
{
    [HarmonyPostfix]
    public static void Postfix(PlayerControl __instance, DeathReason reason)
    {
        // Check if player was murdered
        if (reason != DeathReason.Kill) return;

        DeadBody body = GameObject.FindObjectsOfType<DeadBody>().FirstOrDefault(b => b.ParentId == __instance.PlayerId);
        if (body) NeuroPlugin.Instance.VisionHandler.AddDeadBody(body);
    }
}
