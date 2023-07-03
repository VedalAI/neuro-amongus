using System.Reflection;
using HarmonyLib;

namespace Neuro.Resources.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
public static class AmongUsClient_Awake
{
    [HarmonyPrefix]
    public static void Prefix()
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