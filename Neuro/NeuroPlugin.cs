global using static Reactor.Utilities.Logger<Neuro.NeuroPlugin>;

using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
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
public partial class NeuroPlugin : BasePlugin
{
    public static NeuroPlugin Instance => PluginSingleton<NeuroPlugin>.Instance;

    public MinigamesHandler MinigamesHandler { get; private set; }
    public MovementHandler MovementHandler { get; private set; }
    public PathfindingHandler PathfindingHandler { get; private set; }
    public RecordingHandler RecordingHandler { get; private set; }
    public TasksHandler TasksHandler { get; private set; }
    public VisionHandler VisionHandler { get; private set; }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        MinigamesHandler = new MinigamesHandler();
        MovementHandler = new MovementHandler();
        PathfindingHandler = new PathfindingHandler();
        RecordingHandler = GameObjectUtilities.CreatePermanentSingleton<RecordingHandler>();
        TasksHandler = GameObjectUtilities.CreatePermanentSingleton<TasksHandler>();
        VisionHandler = GameObjectUtilities.CreatePermanentSingleton<VisionHandler>();
    }
}
