using Neuro.Debugging;
using UnityEngine;

namespace Neuro.Pathfinding;

[DebugTab]
public sealed class PathfindingDebugTab : DebugTab
{
    public override string Name => "Pathfinding";

    public override void BuildUI()
    {
        if (!ShipStatus.Instance)
        {
            GUILayout.Label("No ship instance detected");
            return;
        }

        if (GUILayout.Button("Reinitialize Pathfinding"))
            NeuroPlugin.Instance.Pathfinding.Initialize();

        GUILayout.Label($"Size: {NeuroPlugin.Instance.Pathfinding.gridSize}");

        GUILayout.Label($"Density: {NeuroPlugin.Instance.Pathfinding.gridDensity:F6}");
        NeuroPlugin.Instance.Pathfinding.overrideGridDensity = GUILayout.Toggle(NeuroPlugin.Instance.Pathfinding.overrideGridDensity, "Override");
        if (NeuroPlugin.Instance.Pathfinding.overrideGridDensity)
            NeuroPlugin.Instance.Pathfinding.gridDensity = GUILayout.HorizontalSlider(NeuroPlugin.Instance.Pathfinding.gridDensity, 3, 7);

        GUILayout.Label($"Base Width: {NeuroPlugin.Instance.Pathfinding.gridBaseWidth}");
        NeuroPlugin.Instance.Pathfinding.overrideGridBaseWidth = GUILayout.Toggle(NeuroPlugin.Instance.Pathfinding.overrideGridBaseWidth, "Override");
        if (NeuroPlugin.Instance.Pathfinding.overrideGridBaseWidth)
            NeuroPlugin.Instance.Pathfinding.gridBaseWidth = (int)GUILayout.HorizontalSlider(NeuroPlugin.Instance.Pathfinding.gridBaseWidth, 55, 150);
    }
}