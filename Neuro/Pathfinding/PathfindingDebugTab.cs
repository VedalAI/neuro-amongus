using Neuro.Debugging;
using UnityEngine;

namespace Neuro.Pathfinding;

[DebugTab]
public sealed class PathfindingDebugTab : DebugTab
{
    public override string Name => "Pathfinding";

    public override bool IsEnabled => PathfindingHandler.Instance;

    private bool _overrideDensity;
    private float _density;

    private bool _overrideBaseWidth;
    private int _baseWidth;

    public override void BuildUI()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label($"Density: {_density:F6}");
        GUILayout.FlexibleSpace();
        _overrideDensity = GUILayout.Toggle(_overrideDensity, "Override");
        GUILayout.EndHorizontal();

        if (!_overrideDensity)
        {
            GUI.enabled = false;
            _density = PathfindingHandler.Instance.GridDensity;
        }
        _density = GUILayout.HorizontalSlider(_density, 3, 7);
        GUI.enabled = true;

        GUILayout.BeginHorizontal();
        GUILayout.Label($"Base Width: {_baseWidth}");
        GUILayout.FlexibleSpace();
        _overrideBaseWidth = GUILayout.Toggle(_overrideBaseWidth, "Override");
        GUILayout.EndHorizontal();

        if (!_overrideBaseWidth)
        {
            GUI.enabled = false;
            _baseWidth = PathfindingHandler.Instance.GridBaseWidth;
        }
        _baseWidth = Mathf.RoundToInt(GUILayout.HorizontalSlider(_baseWidth, 55, 150));
        GUI.enabled = true;

        GUILayout.Label($"Size: {_baseWidth * _density:F0}");

        if (GUILayout.Button("Rebuild Grid"))
        {
            PathfindingHandler.Instance.InitializeGridSizes();
            if (_overrideDensity) PathfindingHandler.Instance.GridDensity = _density;
            if (_overrideBaseWidth) PathfindingHandler.Instance.GridBaseWidth = _baseWidth;
            PathfindingHandler.Instance.InitializeThread();
        }
    }
}
