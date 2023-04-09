using Neuro.Pathfinding;
using Neuro.Utilities.Convertors;
using UnityEngine;

namespace Neuro.Communication.AmongUsAI.DataStructures;

public readonly struct PositionData
{
    public float TotalDistance { get; init; } = -1;
    public Vector2 OffsetToNextNode { get; init; } = Vector2.zero;

    public PositionData(float totalDistance, Vector2 offsetToNextNode)
    {
        TotalDistance = totalDistance;
        OffsetToNextNode = offsetToNextNode;
    }

    public static readonly PositionData Absent = new();

    public static PositionData Create(PositionProvider position, IdentifierProvider pathfindingIdentifier)
    {
        return new PositionData
        {
            TotalDistance = PathfindingHandler.Instance.CalculateTotalDistance(PlayerControl.LocalPlayer, position, pathfindingIdentifier),
            OffsetToNextNode = PathfindingHandler.Instance.CalculateOffsetToFirstNode(PlayerControl.LocalPlayer, position, pathfindingIdentifier)
        };
    }
}
