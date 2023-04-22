using Neuro.Pathfinding;
using UnityEngine;

namespace Neuro.Recording.Common;

public partial class PositionData
{
    public static PositionData Create(Vector2 position, string pathfindingIdentifier)
    {
        UnityEngine.Vector2 nextNodePos = PathfindingHandler.Instance.GetFirstNodeInPath(position, pathfindingIdentifier);

        return new PositionData
        {
            TotalDistance = PathfindingHandler.Instance.GetPathLength(position, pathfindingIdentifier),
            NextNodePosition = nextNodePos,
            NextNodeOffset = (nextNodePos - PlayerControl.LocalPlayer.GetTruePosition()).normalized
        };
    }

    public static PositionData Create(Vector2 position, int pathfindingIdentifier) => Create(position, pathfindingIdentifier.ToString());

    public static PositionData Create(Component component) => Create(component.transform.position, component.GetInstanceID());
}
