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
using Neuro.Vision;
using Neuro.Impostor;
using Reactor;
using Reactor.Utilities;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin
{
    public static NeuroPlugin Instance => PluginSingleton<NeuroPlugin>.Instance;

    public MinigamesHandler Minigames { get; private set; }
    public MovementHandler Movement { get; private set; }
    public PathfindingHandler Pathfinding { get; private set; }
    public RecordingHandler Recording { get; private set; }
    public TasksHandler Tasks { get; private set; }
    public VisionHandler Vision { get; private set; }
    public ImpostorHandler Impostor { get; private set; }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        // TODO: Maybe reset these when a new game begins.

        Minigames = new MinigamesHandler();
        Movement = new MovementHandler();
        Pathfinding = new PathfindingHandler();
        Recording = AddComponent<RecordingHandler>();
        Tasks = AddComponent<TasksHandler>();
        Vision = AddComponent<VisionHandler>();
        Impostor = AddComponent<ImpostorHandler>();
    }
}
