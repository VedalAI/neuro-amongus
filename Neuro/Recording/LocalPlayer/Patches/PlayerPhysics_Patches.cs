using HarmonyLib;

namespace Neuro.Recording.LocalPlayer.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
public static class PlayerPhysics_RpcEnterVent
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordVent();
    }
}

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcExitVent))]
public static class PlayerPhysics_RpcExitVent
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        LocalPlayerRecorder.Instance.RecordVent();
    }
}
