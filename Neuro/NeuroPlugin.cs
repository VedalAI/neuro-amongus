using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.DependencyInjection;
using Neuro.Utilities;
using Neuro.Vision;
using Reactor;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin, IContextProvider
{
    // Patches use PluginSingleton<NeuroPlugin>.Instance to access this context provider, however in case
    // we want to change it for testing, we should be able to do that by modifying the getter below.
    public IContextProvider MainContext => this;

    public IVisionHandler VisionHandler { get; private set; }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        VisionHandler = GameObjectUtilities.CreatePermanentSingleton<VisionHandler>(this);
    }
}
