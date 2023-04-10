using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neuro.Communication.AmongUsAI;
using Neuro.Utilities;
using Neuro.Vision;
using UnityEngine;

namespace Neuro.Recording.DeadBodies;

public readonly struct DeadBodyData : ISerializable
{
    public byte ParentId { get; init; }
    public Vector2 LastSeenPosition { get; init; }
    public float FirstSeenTime { get; init; }
    public List<byte> NearbyPlayers { get; init; }

    private DeadBodyData(byte parentId, Vector2 lastSeenPosition, float firstSeenTime, List<byte> nearbyPlayers)
    {
        ParentId = parentId;
        LastSeenPosition = lastSeenPosition;
        FirstSeenTime = firstSeenTime;
        NearbyPlayers = nearbyPlayers;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(ParentId);
        writer.Write(LastSeenPosition);
        writer.Write(FirstSeenTime);
        writer.Write((byte) NearbyPlayers.Count);
        foreach (byte playerId in NearbyPlayers)
            writer.Write(playerId);
    }

    public static DeadBodyData Create(DeadBody deadBody)
    {
        List<(int id, float distance)> nearbyPlayers = new();
        foreach (PlayerControl potentialWitness in PlayerControl.AllPlayerControls)
        {
            if (potentialWitness.AmOwner) continue;
            if (potentialWitness.inVent || potentialWitness.Data.IsDead) continue;

            if (!Visibility.IsVisible(potentialWitness)) continue;

            // If a witness is closer to the body than to neuro, there is a chance they did the kill.
            float distanceBetweenWitnessAndBody = Vector2.Distance(potentialWitness.GetTruePosition(), deadBody.TruePosition);
            float distanceBetweenWitnessAndNeuro = Vector2.Distance(potentialWitness.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition());

            if (distanceBetweenWitnessAndBody < distanceBetweenWitnessAndNeuro) nearbyPlayers.Add((potentialWitness.PlayerId, distanceBetweenWitnessAndBody));
        }

        List<byte> nearbyPlayersList = nearbyPlayers.OrderBy(e => e.distance).Select(e => (byte) e.id).ToList();
        return new DeadBodyData(deadBody.ParentId, deadBody.TruePosition, Time.fixedTime, nearbyPlayersList);
    }
}
