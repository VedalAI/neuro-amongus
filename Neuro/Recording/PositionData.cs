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
    public Vector2 NextNodePosition { get; init; } = Vector2.zero;

    public PositionData(float totalDistance, Vector2 nextNodePosition)
    {
        TotalDistance = totalDistance;
        NextNodePosition = nextNodePosition;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(TotalDistance);
        writer.Write(NextNodePosition);
    }

    public static PositionData Create(PositionProvider position, IdentifierProvider pathfindingIdentifier)
    {
        return new PositionData
        {
            TotalDistance = PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, position, pathfindingIdentifier),
            NextNodePosition = PathfindingHandler.Instance.GetFirstNodeInPath(PlayerControl.LocalPlayer, position, pathfindingIdentifier)
        };
    }
}
