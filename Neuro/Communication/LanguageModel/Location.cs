/*
using UnityEngine;

namespace Neuro.Utilities;

public static class Location
{
    public static string GetFromPosition(Vector2 position)
    {
        if (!ShipStatus.Instance) return "the lobby"; // If called from the lobby

        float closestDistance = float.MaxValue;
        PlainShipRoom closestLocation = null;
        string nearPrefix = "outside near "; // If we're not in any rooms/hallways, we're "outside"

        foreach (PlainShipRoom room in ShipStatus.Instance.AllRooms)
        {
            if (room.roomArea && room.roomArea.OverlapPoint(position))
            {
                if (room.RoomId == SystemTypes.Hallway)
                    nearPrefix = "a hallway near "; // keep looking for the nearest room
                else
                    return TranslationController.Instance.GetString(room.RoomId); // If we're inside a proper room, ignore the nearPrefix
            }
            else if (room.RoomId != SystemTypes.Hallway)
            {
                float distance = room.roomArea
                    ? Vector2.Distance(position, room.roomArea.ClosestPoint(position))
                    : Mathf.Infinity;
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = room;
                }
            }
        }

        if (!closestLocation) return "";

        // We're not in an actual room, so say which room we're nearest to
        return nearPrefix + TranslationController.Instance.GetString(closestLocation.RoomId);
    }

    public static string GetLocation(this Vector2 position) => GetFromPosition(position);
    public static string GetLocation(this Vector3 position) => GetFromPosition(position);
}
*/
