using Neuro.Pathfinding;
using Neuro.Utilities.Convertors;
using UnityEngine;

namespace Neuro.Recording;

public partial class PositionData
{
    public static PositionData Create(PositionProvider position, IdentifierProvider pathfindingIdentifier)
    {
        Vector2 nextNodePos = PathfindingHandler.Instance.GetFirstNodeInPath(PlayerControl.LocalPlayer, position, pathfindingIdentifier);

        return new PositionData
        {
            TotalDistance = PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, position, pathfindingIdentifier),
            NextNodePosition = nextNodePos,
            NextNodeOffset = nextNodePos - PlayerControl.LocalPlayer.GetTruePosition()
        };
    }
}
