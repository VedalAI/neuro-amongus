using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.DependencyInjection;
using Neuro.Minigames;
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
public partial class NeuroPlugin : BasePlugin, IContextProvider
{
    public static NeuroPlugin Instance => PluginSingleton<NeuroPlugin>.Instance;

    public IContextProvider MainContext => this;

    public IMinigamesHandler MinigamesHandler { get; private set; }
    public IMovementHandler MovementHandler { get; private set; }
    public IPathfindingHandler PathfindingHandler { get; private set; }
    public IRecordingHandler RecordingHandler { get; private set; }
    public ITasksHandler TasksHandler { get; private set; }
    public IVisionHandler VisionHandler { get; private set; }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        MinigamesHandler = new MinigamesHandler();
        MovementHandler = new MovementHandler();
        PathfindingHandler = new PathfindingHandler();
        RecordingHandler = GameObjectUtilities.CreatePermanentSingleton<RecordingHandler>(MainContext);
        TasksHandler = GameObjectUtilities.CreatePermanentSingleton<TasksHandler>(MainContext);
        VisionHandler = GameObjectUtilities.CreatePermanentSingleton<VisionHandler>(MainContext);
    }
}
