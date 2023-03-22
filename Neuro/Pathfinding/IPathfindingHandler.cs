using Neuro.DependencyInjection;

namespace Neuro.Pathfinding;

public interface IPathfindingHandler : IContextAcceptor
{
    public void Initialize();
}
