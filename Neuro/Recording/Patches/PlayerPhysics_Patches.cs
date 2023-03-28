using HarmonyLib;

namespace Neuro.Recording.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
public static class PlayerPhysics_RpcEnterVent
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        NeuroPlugin.Instance.Recording.DidVent = true;
    }
}

// TODO: Consider exiting a vent as venting
