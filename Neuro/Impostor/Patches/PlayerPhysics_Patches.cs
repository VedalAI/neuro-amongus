using HarmonyLib;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.RpcEnterVent))]
public static class PlayerPhysics_RpcEnterVent
{
    [HarmonyPostfix]
    public static void Postfix(int id)
    {
        ImpostorHandler.Instance.HandleVent(id);
    }
}
