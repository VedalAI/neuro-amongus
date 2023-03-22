using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Neuro.Utils;

public static class Methods
{
    private static Dictionary<Vector2, string> locationNames = new Dictionary<Vector2, string>()
    {
        { new Vector2 { x = 0, y = 0 }, "Cafeteria" },
        { new Vector2 { x = 9, y = 1 }, "Weapons" },
        { new Vector2 { x = 6.6f, y = -3.7f }, "O2" },
        { new Vector2 { x = 17, y = -5 }, "Navigation" },
        { new Vector2 { x = 9, y = -12.5f }, "Shields" },
        { new Vector2 { x = 4, y = 15.6f }, "Communications" },
        { new Vector2 { x = 0, y = -17 }, "Trash Chute" },
        { new Vector2 { x = 0, y = -12 }, "Storage" },
        { new Vector2 { x = -9, y = -10f }, "Electrical" },
        { new Vector2 { x = -15.6f, y = -10.6f }, "Lower Engine" },
        { new Vector2 { x = -13f, y = -4.4f }, "Security" },
        { new Vector2 { x = -21f, y = -5.5f }, "Reactor" },
        { new Vector2 { x = -15.4f, y = 1 }, "Upper Engine" },
        { new Vector2 { x = -8f, y = -3.5f }, "MedBay" },
        { new Vector2 { x = 5f, y = -8 }, "Admin" },
    };

    public static string GetLocationFromPosition(Vector2 position)
    {
        float closestDistance = Mathf.Infinity;
        string closestLocation = "";

        foreach (KeyValuePair<Vector2, string> keyValuePair in locationNames)
        {
            float distance = Vector2.Distance(keyValuePair.Key, position);
            if (distance < 2f)
            {
                return keyValuePair.Value;
            }
            else
            {
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestLocation = keyValuePair.Value;
                }
            }
        }

        return closestLocation;
    }
}
