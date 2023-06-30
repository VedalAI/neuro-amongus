using HarmonyLib;
using Rewired;
using UnityEngine;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Application), nameof(Application.isFocused), MethodType.Getter)]
public static class Application_get_isFocused
{
    [HarmonyPrefix]
    public static bool Prefix(out bool __result)
    {
        __result = true;
        return false;
    }
}
