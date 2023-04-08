using Neuro.Pathfinding;
using Neuro.Utilities;
using UnityEngine;
using UnityEngine.UIElements;

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

    public static PositionData Create(PositionProvider position)
    {
        return new PositionData
        {
            TotalDistance = PathfindingHandler.Instance.CalculateTotalDistance(PlayerControl.LocalPlayer.GetTruePosition(), position),
            OffsetToNextNode = PathfindingHandler.Instance.CalculateOffsetToFirstNode(PlayerControl.LocalPlayer.GetTruePosition(), position)
        };
    }
}
