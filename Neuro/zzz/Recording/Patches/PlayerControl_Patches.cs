using HarmonyLib;

namespace Neuro.Recording.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
public static class PlayerControl_CmdCheckMurder
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!Recorder.Instance) return;
        Recorder.Instance.DidKill = true;
    }
}

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdReportDeadBody))]
public static class PlayerControl_CmdReportDeadBody
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!Recorder.Instance) return;
        Recorder.Instance.DidReport = true;
    }
}
