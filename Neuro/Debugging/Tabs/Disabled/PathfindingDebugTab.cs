using Neuro.Pathfinding;
using UnityEngine;

namespace Neuro.Debugging.Tabs.Disabled;

// [DebugTab]
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
        using (new HorizontalScope())
        {
            GUILayout.Label($"Density: {_density:F6}");
            GUILayout.FlexibleSpace();
            _overrideDensity = GUILayout.Toggle(_overrideDensity, "Override");
        }

        using (new DisabledScope(!_overrideDensity))
        {
            if (!_overrideDensity) _density = PathfindingHandler.Instance.GridDensity;
            _density = GUILayout.HorizontalSlider(_density, 3, 7);
        }

        using (new HorizontalScope())
        {
            GUILayout.Label($"Base Width: {_baseWidth}");
            GUILayout.FlexibleSpace();
            _overrideBaseWidth = GUILayout.Toggle(_overrideBaseWidth, "Override");
        }

        using (new DisabledScope(!_overrideBaseWidth))
        {
            if (!_overrideBaseWidth) _baseWidth = PathfindingHandler.Instance.GridBaseWidth;
            _baseWidth = Mathf.RoundToInt(GUILayout.HorizontalSlider(_baseWidth, 55, 150));
        }

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
