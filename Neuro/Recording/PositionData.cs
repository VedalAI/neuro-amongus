using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Pathfinding;
using Neuro.Utilities;
using Neuro.Utilities.Convertors;
using UnityEngine;

namespace Neuro.Recording;

public readonly struct PositionData : ISerializable
{
    public float TotalDistance { get; init; } = -1;
    public Vector2 OffsetToNextNode { get; init; } = Vector2.zero;

    public PositionData(float totalDistance, Vector2 offsetToNextNode)
    {
        TotalDistance = totalDistance;
        OffsetToNextNode = offsetToNextNode;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(TotalDistance);
        writer.Write(OffsetToNextNode);
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
