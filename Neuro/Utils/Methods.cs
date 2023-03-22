using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Utils;

public static class Methods
{
    public static string GetLocationFromPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        PlainShipRoom closestLocation = null;
        string nearPrefix = "outside near "; // If we're not in any rooms/hallways, we're "outside"

        if (!ShipStatus.Instance) // In case this is called from the lobby
            return "the lobby";

        foreach (PlainShipRoom room in ShipStatus.Instance.AllRooms)
        {
            Collider2D collider = room.roomArea;
            if (collider.OverlapPoint(position))
            {
                if (room.RoomId == SystemTypes.Hallway)
                    nearPrefix = "a hallway near "; // keep looking for the nearest room
                else
                    return room.DisplayName(); // If we're inside a proper room, ignore the nearPrefix
            }
            else if (room.RoomId != SystemTypes.Hallway)
            {
                float distance = Vector2.Distance(position, collider.ClosestPoint(position));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = room;
                }
            }
        }

        if (!closestLocation)
            return "";

        // We're not in an actual room, so say which room we're nearest to
        return nearPrefix + closestLocation.DisplayName();
    }

    // Gets the name of a room, as displayed by the game's user interface
    public static string DisplayName (this PlainShipRoom room)
    {
        return TranslationController.Instance.GetString(room.RoomId);
    }
}
