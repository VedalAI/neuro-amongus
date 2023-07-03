using System.Reflection;
using HarmonyLib;

namespace Neuro.Resources.Patches;

[HarmonyPatch]
public static class LoadResourcesPatches
{
    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
    [HarmonyPrefix]
    public static void LoadOxygenFilterPatch()
    {
        try
        {
            Assembly.Load("Reactor.OxygenFilter").GetType("Reactor.ReactorUtilities")!.GetMethod("RegisterAssembly")!.Invoke(null, new object[] {Assembly.GetExecutingAssembly()});
        }
        catch
        {
            // idc
        }
    }
}
