using HarmonyLib;
using BepInEx.Unity.IL2CPP.Utils;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
public static class PlayerPhysics_RpcEnterVent
{
    [HarmonyPostfix]
    public static void Postfix()
    {
        PlayerControl.LocalPlayer.StartCoroutine(NeuroPlugin.Instance.Impostor.CoStartVentOut(NeuroPlugin.Instance.Impostor.ClosestVent));
    }
}
