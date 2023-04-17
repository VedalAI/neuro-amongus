using Neuro.Debugging;
using Neuro.Utilities;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Recording.LocalPlayer;

[DebugTab]
public sealed class ObstaclesDebugTab : DebugTab
{
    public override string Name => "Obstacles";
    public override bool IsEnabled => LocalPlayerRecorder.Instance && PlayerControl.LocalPlayer;

    private readonly LineRenderer[] _lineRenderers = new LineRenderer[8];

    private bool _enableLineRenderers;

    public override void BuildUI()
    {
        _enableLineRenderers = GUILayout.Toggle(_enableLineRenderers, "Enable Raycast Lines");
    }

    public override void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            GameObject obj = new("Player Raycast Renderer");
            obj.DontDestroy();

            LineRenderer line = obj.AddComponent<LineRenderer>();
            line.SetPosition(0, Vector3.zero);
            line.SetPosition(1, Vector3.zero);
            line.widthMultiplier = 0.1f;
            line.positionCount = 2;
            line.material = NeuroUtilities.MaskShaderMat;
            line.startColor = Color.red;
            line.endColor = Color.red;

            _lineRenderers[i] = line;
        }
    }

    public override void OnDisable()
    {
        for (int i = 0; i < 8; i++)
        {
            _lineRenderers[i].enabled = false;
        }
    }

    public override void Update()
    {
        Vector2 playerPos = PlayerControl.LocalPlayer.GetTruePosition();

        for (int i = 0; i < 8; i++)
        {
            LineRenderer lineRenderer = _lineRenderers[i];
            lineRenderer.enabled = _enableLineRenderers;

            lineRenderer.SetPosition(0, playerPos);
            lineRenderer.SetPosition(1, playerPos + LocalPlayerRecorder.RaycastDirections[i].normalized * LocalPlayerRecorder.Instance.Frame.RaycastObstacleDistances[i]);
        }
    }
}
