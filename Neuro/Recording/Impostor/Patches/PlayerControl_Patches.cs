using HarmonyLib;

namespace Neuro.Recording.Impostor.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CmdCheckMurder))]
public static class PlayerControl_CmdCheckMurder
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        ImpostorRecorder.Instance.RecordKill();
    }
}
