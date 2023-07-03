using Neuro.Caching;
using Neuro.Pathfinding;
using Neuro.Recording.LocalPlayer;
using Reactor.Utilities.Extensions;
using UnityEngine;

namespace Neuro.Debugging.Tabs;

[DebugTab]
public sealed class GizmosDebugTab : DebugTab
{
    public override string Name => "Gizmos";
    public override bool IsEnabled => PlayerControl.LocalPlayer;

    private readonly LineRenderer[] _obstacleRenderers = new LineRenderer[8];

    public static bool EnableTaskPaths
    {
        get => CachedPlayerPrefs.GetBool("EnableTaskPaths", true);
        set => CachedPlayerPrefs.SetBool("EnableTaskPaths", value);
    }

    private static bool _enableNodes
    {
        get => CachedPlayerPrefs.GetBool("EnableNodes", true);
        set => CachedPlayerPrefs.SetBool("EnableNodes", value);
    }

    private static bool _enableObstacleRaycasts
    {
        get => CachedPlayerPrefs.GetBool("EnableObstacles", false);
        set => CachedPlayerPrefs.SetBool("EnableObstacles", value);
    }

    public override void BuildUI()
    {
        using (new HorizontalScope())
        {
            _enableNodes = GUILayout.Toggle(_enableNodes, "Pathfinding Nodes", GUI.skin.button);
            EnableTaskPaths = GUILayout.Toggle(EnableTaskPaths, "Task Paths", GUI.skin.button);
            _enableObstacleRaycasts = GUILayout.Toggle(_enableObstacleRaycasts, "Obstacle Raycasts", GUI.skin.button);
        }
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
            line.material = UnityCache.MaskShaderMat;
            line.startColor = Color.red;
            line.endColor = Color.red;

            _obstacleRenderers[i] = line;
        }
    }

    public override void OnDisable()
    {
        for (int i = 0; i < 8; i++)
        {
            _obstacleRenderers[i].enabled = false;
        }
    }

    public override void Update()
    {
        if (PathfindingHandler.Instance)
        {
            PathfindingHandler.Instance.VisualPointParent.gameObject.SetActive(_enableNodes);
        }

        if (LocalPlayerRecorder.Instance)
        {
            Vector2 playerPos = PlayerControl.LocalPlayer.GetTruePosition();

            for (int i = 0; i < 8; i++)
            {
                LineRenderer lineRenderer = _obstacleRenderers[i];
                lineRenderer.enabled = _enableObstacleRaycasts;

                lineRenderer.SetPosition(0, playerPos);
                lineRenderer.SetPosition(1, playerPos + LocalPlayerRecorder.RaycastDirections[i].normalized * LocalPlayerRecorder.Instance.Frame.RaycastObstacleDistances[i]);
            }
        }
    }
}