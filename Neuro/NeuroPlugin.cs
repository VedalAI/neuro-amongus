global using static Reactor.Utilities.Logger<Neuro.NeuroPlugin>;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.Debugging;
using Neuro.Utilities;
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

        AddComponent<DebugWindow>();

        ResourceManager.CacheSprite("Cursor", 130);
    }
}
