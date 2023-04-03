using HarmonyLib;

namespace Neuro.Recording.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
public static class PlayerPhysics_RpcEnterVent
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        if (!Recorder.Instance) return;
        Recorder.Instance.DidVent = true;
    }
}
