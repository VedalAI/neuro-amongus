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

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
public static class PlayerControl_CmdCheckMurder
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordKill();
    }
}
