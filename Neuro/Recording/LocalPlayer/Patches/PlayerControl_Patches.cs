using HarmonyLib;

namespace Neuro.Recording.LocalPlayer.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
public static class PlayerControl_CmdReportDeadBody
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordReport();
    }
}
