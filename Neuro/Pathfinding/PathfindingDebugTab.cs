using Neuro.Debugging;
using UnityEngine;

namespace Neuro.Pathfinding;

[DebugTab]
public sealed class PathfindingDebugTab : DebugTab
{
    public override string Name => "Pathfinding";

    public override bool IsEnabled => ShipStatus.Instance && PathfindingHandler.Instance;

    public override void BuildUI()
    {
        foreach (PlainDoor door in ShipStatus.Instance.AllDoors)
        {
            IdentifierProvider identifier = door;
            GUILayout.Label($"{identifier}: {PathfindingHandler.Instance.CalculateTotalDistance(PlayerControl.LocalPlayer, door, identifier)}", GUILayout.Width(150));
        }
    }
}
