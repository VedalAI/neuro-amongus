using Neuro.Debugging;
using UnityEngine;

namespace Neuro.Pathfinding;

[DebugTab]
public sealed class PathfindingDebugTab : DebugTab
{
    public override string Name => "Pathfinding";
    public override void BuildUI()
    {
        if (ShipStatus.Instance && GUILayout.Button("Regenerate Grid"))
        {
            NeuroPlugin.Instance.Pathfinding.Initialize();
        }

        GUILayout.Label($"Density: {NeuroPlugin.Instance.Pathfinding.gridDensity:F5}");
        NeuroPlugin.Instance.Pathfinding.gridDensity = GUILayout.HorizontalSlider(NeuroPlugin.Instance.Pathfinding.gridDensity, 3, 7);

        GUILayout.Label($"Base Width: {NeuroPlugin.Instance.Pathfinding.gridBaseWidth}");
        NeuroPlugin.Instance.Pathfinding.gridBaseWidth = (int)GUILayout.HorizontalSlider(NeuroPlugin.Instance.Pathfinding.gridBaseWidth, 50, 100);
    }
}