using Neuro.Arrow;
using Neuro.Movement;
using Neuro.Pathfinding;
using Neuro.Recording;
using Neuro.Tasks;
using Neuro.Visibility;
using Neuro.Vision;

namespace Neuro.DependencyInjection;

public interface IContextProvider
{
    public IArrowHandler ArrowHandler { get; }
    public IMovementHandler MovementHandler { get; }
    public IPathfindingHandler PathfindingHandler { get; }
    public IRecordingHandler RecordingHandler { get; }
    public ITasksHandler TasksHandler { get; }
    public IVisibilityHandler VisibilityHandler { get; }
    public IVisionHandler VisionHandler { get; }
}
