global using static Reactor.Utilities.Logger<Neuro.NeuroPlugin>;
using System.Diagnostics;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.Communication.AmongUsAI;
using Neuro.Debugging;
using Neuro.Minigames.Solvers;
using Neuro.Recording;
using Neuro.Resources;
using Reactor;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin
{
    static NeuroPlugin()
    {
        DependencyResolver.InjectResources();
    }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        AddFullComponents();
        AddComponent<Uploader>();

        ResourceManager.CacheSprite("Cursor", 130);
    }

    [Conditional("FULL")]
    private void AddFullComponents()
    {
        AddComponent<CommunicationHandler>();
        AddComponent<DebugWindow>();
        AddComponent<something>();
    }
}