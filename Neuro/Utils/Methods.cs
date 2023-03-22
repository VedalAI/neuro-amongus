using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Neuro.Utils;

public static class Methods
{
    public static string GetLocationFromPosition(Vector2 position, bool includeHallways = false)
    {
        float closestDistance = Mathf.Infinity;
        PlainShipRoom closestLocation = null;

        if (ShipStatus.Instance == null) // In case this is called from the lobby
            return "";

        foreach (PlainShipRoom room in ShipStatus.Instance.AllRooms)
        {
            // Only include actual rooms (not hallways), if required
            if (!includeHallways && room.RoomId == SystemTypes.Hallway)
                continue;

            Collider2D collider = room.roomArea;
            if (collider.OverlapPoint(position))
            {
                return room.DisplayName();
            }
            else
            {
                float distance = Vector2.Distance(position, collider.ClosestPoint(position));
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = room;
                }
            }
        }
        
        return closestLocation.DisplayName();
    }

    // Gets the name of a room, as displayed by the game's user interface
    // Calling PlainShipRoom.RoomId.ToString() usually gives the correct name, but there are a few rooms for which it
    // gives a different name than the game's UI shows (i.e., "LifeSupp" instead of "O2"); this function corrects this
    public static string DisplayName (this PlainShipRoom room)
    {
        switch (room.RoomId)
        {
            case SystemTypes.LifeSupp: return "O2";
            case SystemTypes.Nav: return "Navigation";
            case SystemTypes.Decontamination2: return "Upper Decontamination"; // Used on Polus
            case SystemTypes.Decontamination3: return "Lower Decontamination"; // Used on Polus
            default: return Regex.Replace(room.RoomId.ToString(), "(\\B[A-Z])", " $1"); // Adds spaces to CamelCase strings
        }
    }
}
