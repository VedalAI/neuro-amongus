using HarmonyLib;
using System.Collections;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using Neuro.Extensions.Harmony;

namespace Neuro.Impostor.Patches;

[FullHarmonyPatch]
public static class VentingPatches
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoEnterVent))]
    [HarmonyPostfix]
    public static void Postfix(PlayerPhysics __instance, ref Il2CppSystem.Collections.IEnumerator __result)
    {
        if (!__instance.AmOwner) return;
        __result = PostfixedEnumerator(__result).WrapToIl2Cpp();
    }

    private static IEnumerator PostfixedEnumerator(Il2CppSystem.Collections.IEnumerator original)
    {
        yield return original;
        yield return ImpostorHandler.Instance.CoStartVentOut();
    }
}
