using Neuro.Pathfinding;
using Neuro.Tasks;
using Neuro.Visibility;
using Neuro.Vision;

namespace Neuro.DependencyInjection;

public interface IContextProvider
{
    public IPathfindingHandler PathfindingHandler { get; }
    public ITasksHandler TasksHandler { get; }
    public IVisibilityHandler VisibilityHandler { get; }
    public IVisionHandler VisionHandler { get; }
}
