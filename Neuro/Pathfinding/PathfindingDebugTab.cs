using System.Linq;
using Neuro.Debugging;
using Neuro.Utilities.Convertors;
using UnityEngine;

namespace Neuro.Pathfinding;

[DebugTab]
public sealed class PathfindingDebugTab : DebugTab
{
    public override string Name => "Pathfinding";

    public override bool IsEnabled => PathfindingHandler.Instance;

    public override void BuildUI()
    {
        foreach (PlainDoor door in ShipStatus.Instance.AllDoors)
        {
            IdentifierProvider identifier = door;
            GUILayout.Label($"{identifier}: {PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, door, identifier)}", GUILayout.Width(175));
        }

        foreach (Vent vent in ShipStatus.Instance.AllVents.OrderBy(v => v.Id))
        {
            IdentifierProvider identifier = vent;
            GUILayout.Label($"{identifier}: {PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, vent, identifier)}", GUILayout.Width(175));
        }
    }
}
