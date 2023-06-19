using HarmonyLib;
using System.Collections;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils.Collections;

namespace Neuro.Impostor.Patches;

[HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.CoEnterVent))]
public static class PlayerPhysics_CoEnterVent
{
    [HarmonyPostfix]
    public static void Postfix(ref Il2CppSystem.Collections.IEnumerator __result)
    {
        __result = PostfixedEnumerator(__result).WrapToIl2Cpp();
    }

    private static IEnumerator PostfixedEnumerator(Il2CppSystem.Collections.IEnumerator original)
    {
        yield return original;
        yield return ImpostorHandler.Instance.CoStartVentOut();
    }
}
