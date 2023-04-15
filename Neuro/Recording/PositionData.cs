using Neuro.Pathfinding;
using Neuro.Utilities.Convertors;

namespace Neuro.Recording;

public partial class PositionData
{
    public static PositionData Create(PositionProvider position, IdentifierProvider pathfindingIdentifier)
    {
        return new PositionData
        {
            TotalDistance = PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, position, pathfindingIdentifier),
            NextNodePosition = PathfindingHandler.Instance.GetFirstNodeInPath(PlayerControl.LocalPlayer, position, pathfindingIdentifier) - PlayerControl.LocalPlayer.GetTruePosition()
        };
    }
}
