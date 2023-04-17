using HarmonyLib;

namespace Neuro.Debugging.Patches;

[HarmonyPatch(typeof(LogicGameFlowNormal), nameof(LogicGameFlowNormal.CheckEndCriteria))]
public static class LogicGameFlowNormal_CheckEndCriteria
{
    [HarmonyPrefix]
    public static bool Prefix()
    {
        return false;
    }
}
