using HarmonyLib;

namespace Neuro.Recording.LocalPlayer.Patches;

[HarmonyPatch]
public static class PlayerActionsRecordingPatches
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
    [HarmonyPostfix]
    public static void RecordReportPatch()
    {
        LocalPlayerRecorder.Instance.RecordReport();
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
    [HarmonyPostfix]
    public static void RecordMurderPatch()
    {
        LocalPlayerRecorder.Instance.RecordKill();
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.UseClosest))]
    [HarmonyPrefix]
    public static void RecordInteractPatch(PlayerControl __instance) // only invoked for local player
    {
        if (__instance.closest != null && LocalPlayerRecorder.Instance)
        {
            LocalPlayerRecorder.Instance.RecordInteract();
        }
    }

    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcExitVent))]
    [HarmonyPostfix]
    public static void RecordVentPatch()
    {
        LocalPlayerRecorder.Instance.RecordVent();
    }
}