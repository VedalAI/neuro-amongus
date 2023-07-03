using System.Collections.Generic;
using System.Linq;
using Google.Protobuf;
using UnityEngine;

namespace Neuro.Recording.DeadBodies;

public partial class DeadBodyData
{
    public static DeadBodyData Create(DeadBody deadBody)
    {
        List<(int id, float distance)> nearbyPlayers = new();
        foreach (PlayerControl potentialWitness in PlayerControl.AllPlayerControls)
        {
            if (potentialWitness.AmOwner) continue;
            if (potentialWitness.inVent || potentialWitness.Data.IsDead) continue;

            if (!Visibility.IsVisible(potentialWitness.GetTruePosition())) continue;

            // If a witness is closer to the body than to neuro, there is a chance they did the kill.
            float distanceBetweenWitnessAndBody = Vector2.Distance(potentialWitness.GetTruePosition(), deadBody.TruePosition);
            float distanceBetweenWitnessAndNeuro = Vector2.Distance(potentialWitness.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition());

            if (distanceBetweenWitnessAndBody < distanceBetweenWitnessAndNeuro) nearbyPlayers.Add((potentialWitness.PlayerId, distanceBetweenWitnessAndBody));
        }

        byte[] nearbyPlayersArray = nearbyPlayers.OrderBy(e => e.distance).Select(e => (byte) e.id).ToArray();
        return new DeadBodyData
        {
            ParentId = deadBody.ParentId,
            Position = deadBody.TruePosition,
            FirstSeenTime = Time.fixedTime,
            NearbyPlayers = ByteString.CopyFrom(nearbyPlayersArray)
        };
    }
}