using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Utilities;

public static class Gizmos
{
    static Gizmos()
    {
        _nodeMaterial = new Material(Shader.Find("Unlit/MaskShader"));
        _nodeMaterial.DontDestroy();
    }

    private static readonly Material _nodeMaterial;
    private static GameObject _previousPath;

    public static void CreateNodeVisualPoint(Vector2 position) => CreateVisualPoint(position, Color.red, 0.1f);

    public static void CreateDestinationVisualPoint(Vector2 position) => CreateVisualPoint(position, Color.blue, 0.3f);

    public static void DrawPath(Vector2[] path)
    {
        if (_previousPath) _previousPath.Destroy();

        _previousPath = new GameObject("Gizmo (Path)");
        _previousPath.transform.position = PlayerControl.LocalPlayer.transform.position;

        LineRenderer renderer = _previousPath.AddComponent<LineRenderer>();
        renderer.positionCount = path.Length;
        for (int i = 0; i < path.Length; i++)
        {
            Info(path[i].ToString());
            renderer.SetPosition(i, path[i]);
        }

        renderer.widthMultiplier = 0.2f;
        renderer.startColor = Color.blue;
    }

    private static void CreateVisualPoint(Vector2 position, Color color, float widthMultiplier)
    {
        GameObject nodeVisualPoint = new("Gizmo (Visual Point)");
        nodeVisualPoint.transform.position = position;

        LineRenderer renderer = nodeVisualPoint.AddComponent<LineRenderer>();
        renderer.SetPosition(0, position);
        renderer.SetPosition(1, position + new Vector2(0, widthMultiplier));
        renderer.widthMultiplier = widthMultiplier;
        renderer.positionCount = 2;
        renderer.material = _nodeMaterial;
        renderer.startColor = color;
        renderer.endColor = color;
    }
}
