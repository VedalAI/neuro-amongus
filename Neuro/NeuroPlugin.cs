global using static Reactor.Utilities.Logger<Neuro.NeuroPlugin>;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.Communication.AmongUsAI;
using Neuro.Debugging;
using Neuro.Utilities;
using Reactor;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin
{
    // If this is false, we just do the recording and no AI
    public static bool Neuro = false;
    // Used for drawing some debug information
    public static bool Debug = false;

    static NeuroPlugin()
    {
        DependencyResolver.InjectResources();
    }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        if(Neuro)
        {
            AddComponent<CommunicationHandler>();
            AddComponent<DebugWindow>();
        }

        ResourceManager.CacheSprite("Cursor", 130);
    }
}
