global using static Reactor.Utilities.Logger<Neuro.NeuroPlugin>;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.Debugging;
using Neuro.Utilities;
using Reactor;
using Reactor.Utilities;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin
{
    public static NeuroPlugin Instance => PluginSingleton<NeuroPlugin>.Instance;

    // public MovementHandler Movement { get; private set; }
    // public ImpostorHandler Impostor { get; private set; }
    // public CommunicationHandler Communication { get; private set; }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        AddComponent<DebugWindow>();
        // AddComponent<Recorder>();

        // Movement = new MovementHandler();
        // Impostor = AddComponent<ImpostorHandler>();
        // Communication = AddComponent<CommunicationHandler>();

        ResourceManager.CacheSprite("Cursor", 130);
    }
}
