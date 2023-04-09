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
    private byte ParentId { get; init; }
    private Vector2 LastSeenPosition { get; init; }
    private float FirstSeenTime { get; init; }
    private int[] NearbyPlayers { get; init; }

    private DeadBodyData(byte parentId, Vector2 lastSeenPosition, float firstSeenTime, int[] nearbyPlayers)
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
        for (int i = 0; i < 3; i++)
            writer.Write(NearbyPlayers.Take(i..(i+1)).DefaultIfEmpty(-1).ElementAt(i));
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

        int[] nearbyPlayersArray = nearbyPlayers.OrderBy(e => e.distance).Take(3).Select(e => e.id).ToArray();
        return new DeadBodyData(deadBody.ParentId, deadBody.TruePosition, Time.fixedTime, nearbyPlayersArray);
    }
}
