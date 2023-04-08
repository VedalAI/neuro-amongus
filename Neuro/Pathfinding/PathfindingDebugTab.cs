using Neuro.Debugging;
using UnityEngine;

namespace Neuro.Pathfinding;

[DebugTab]
public sealed class PathfindingDebugTab : DebugTab
{
    private bool _densityOverride = false;
    private bool _baseWidthOverride = false;
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
        _densityOverride = GUILayout.Toggle(_densityOverride, "Override");
        if (_densityOverride)
            NeuroPlugin.Instance.Pathfinding.gridDensity = GUILayout.HorizontalSlider(NeuroPlugin.Instance.Pathfinding.gridDensity, 3, 7);

        GUILayout.Label($"Base Width: {NeuroPlugin.Instance.Pathfinding.gridBaseWidth}");
        _baseWidthOverride = GUILayout.Toggle(_baseWidthOverride, "Override");
        if (_baseWidthOverride)
            NeuroPlugin.Instance.Pathfinding.gridBaseWidth = (int)GUILayout.HorizontalSlider(NeuroPlugin.Instance.Pathfinding.gridBaseWidth, 55, 150);
    }
}