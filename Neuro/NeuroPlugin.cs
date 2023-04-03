global using static Reactor.Utilities.Logger<Neuro.NeuroPlugin>;
using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.Debugging;
using Neuro.Movement;
using Neuro.Pathfinding;
using Neuro.Recording;
using Neuro.Tasks;
using Neuro.Utilities;
using Neuro.Vision;
using Reactor;
using Reactor.Utilities;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin
{
    public static NeuroPlugin Instance => PluginSingleton<NeuroPlugin>.Instance;

    public MovementHandler Movement { get; private set; }
    public PathfindingHandler Pathfinding { get; private set; }
    public TasksHandler Tasks { get; private set; }
    public VisionHandler Vision { get; private set; }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        // TODO: Maybe reset these when a new game begins.

        AddComponent<DebugWindow>();
        // AddComponent<Recorder>();


        Movement = new MovementHandler();
        Pathfinding = new PathfindingHandler();
        Tasks = AddComponent<TasksHandler>();
        Vision = AddComponent<VisionHandler>();

        ResourceManager.CacheSprite("Cursor", 130);
    }
}
