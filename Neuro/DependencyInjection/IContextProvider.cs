using Neuro.Arrow;
using Neuro.Minigames;
using Neuro.Movement;
using Neuro.Pathfinding;
using Neuro.Recording;
using Neuro.Tasks;
using Neuro.Vision;

namespace Neuro.DependencyInjection;

public interface IContextProvider
{
    public IArrowHandler ArrowHandler { get; }
    public IMinigamesHandler MinigamesHandler { get; }
    public IMovementHandler MovementHandler { get; }
    public IPathfindingHandler PathfindingHandler { get; }
    public IRecordingHandler RecordingHandler { get; }
    public ITasksHandler TasksHandler { get; }
    public IVisionHandler VisionHandler { get; }
}
