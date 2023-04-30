using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Pathfinding.DataStructures;
using Neuro.Recording.Common;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Neuro.Pathfinding;

[RegisterInIl2Cpp]
public sealed class PathfindingHandler : MonoBehaviour
{
    public float GridDensity { get; set; }
    public int GridBaseWidth { get; set; }

    public int GridSize => (int)(GridBaseWidth * GridDensity);
    public int GridLowerBounds => Mathf.RoundToInt(GridSize / -2f);
    public int GridUpperBounds => Mathf.RoundToInt(GridSize / 2f);

    public static PathfindingHandler Instance { get; private set; }

    public PathfindingHandler(IntPtr ptr) : base(ptr) { }

    private PathfindingThread _thread;
    private GameObject _visualPointParent;

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;

        InitializeGridSizes();
        InitializeThread();
    }

    private void OnDestroy()
    {
        _thread.Stop();
    }

    public void InitializeGridSizes()
    {
        switch (ShipStatus.Instance.GetTypeForMessage())
        {
            case MapType.Dleks:
            case MapType.Skeld:
                GridDensity = 4.489812374114990234375f;
                GridBaseWidth = 58;
                break;

            case MapType.MiraHq:
                GridDensity = 4.404010295867919921875f;
                GridBaseWidth = 80;
                break;

            case MapType.Polus:
                GridDensity = 4.5252170562744140625f;
                GridBaseWidth = 96;
                break;

            case MapType.Airship:
                GridDensity = 4.333333f;
                GridBaseWidth = 100;
                break;

            default:
                Warning("The map requested lacks fine-tuned grid sizes! Defaulting to backup values.");
                GridDensity = 4.5f;
                GridBaseWidth = 100;
                break;
        }
    }

    public void InitializeThread()
    {
        if (_visualPointParent) _visualPointParent.Destroy();
        _visualPointParent = new GameObject("Visual Point Parent");

        _thread?.Stop();
        _thread = new PathfindingThread(GenerateNodeGrid(), ShipStatus.Instance.MeetingSpawnCenter + Vector2.down * ShipStatus.Instance.SpawnRadius, _visualPointParent.transform);
        _thread.Start();
    }

    [HideFromIl2Cpp]
    private Node[,] GenerateNodeGrid()
    {
        Node[,] grid = new Node[GridSize, GridSize];

        const float OFFSET = 1 / 5f; // Must be less than 1 / 4f or it will flood fill through walls
        Vector2[] offsetCoords =
        {
            new(-OFFSET, -OFFSET), new(0, -OFFSET), new(OFFSET, -OFFSET),
            new(-OFFSET, 0), Vector2.zero, new(OFFSET, 0),
            new(-OFFSET, OFFSET), new(0, OFFSET), new(OFFSET, OFFSET)
        };

        for (int x = 0; x < GridSize; x++)
        for (int y = 0; y < GridSize; y++)
        {
            Vector2 point = Vector2.zero;
            bool accessible = false;
            for (int i = 0; i < 9; i++)
            {
                int b = (i * 4 + 4) % 9; // Noncontinuous linear index through the array
                if (TryGetAccessiblePoint(x - GridUpperBounds + offsetCoords[b].x, y - GridUpperBounds + offsetCoords[b].y, out point))
                {
                    accessible = true;
                    break;
                }
            }

            grid[x, y] = new Node(accessible, point, x, y);
        }

        return grid;
    }

    private bool TryGetAccessiblePoint(float x, float y, out Vector2 point)
    {
        float nodeRadius = 1 / GridDensity;
        point = new Vector2(x / GridDensity, y / GridDensity);

        Collider2D[] cols = Physics2D.OverlapCircleAll(point, nodeRadius, Constants.ShipAndAllObjectsMask);
        int validColsCount = cols.Count(col =>
            !col.isTrigger &&
            !col.GetComponentInParent<Vent>() &&
            !col.GetComponentInParent<SomeKindaDoor>()
        );

        // TODO: Add edge case for Airship ladders

        return validColsCount == 0;
    }

    private float GetPathLength(Vector2 start, Vector2 target, string identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("Identifier cannot be null or empty");

        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out _, out float length)) return -1;

        return length;
    }

    public float GetPathLength(Vector2 target, string identifier) => GetPathLength(PlayerControl.LocalPlayer.GetTruePosition(), target, identifier);

    public float GetPathLength(Vector2 target, int identifier) => GetPathLength(target, identifier.ToString());

    public float GetPathLength(Component target) => GetPathLength(target.transform.position, target.GetInstanceID());

    private Vector2 GetFirstNodeInPath(Vector2 start, Vector2 target, string identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("Identifier cannot be null or empty");

        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out Vector2[] path, out _)) return Vector2.zero;

        if (path.Length == 0) return Vector2.zero;

        return path[0];
    }

    public Vector2 GetFirstNodeInPath(Vector2 target, string identifier) => GetFirstNodeInPath(PlayerControl.LocalPlayer.GetTruePosition(), target, identifier);

    public Vector2 GetFirstNodeInPath(Vector2 target, int identifier) => GetFirstNodeInPath(target, identifier.ToString());

    public Vector2 GetFirstNodeInPath(Component target) => GetFirstNodeInPath(target.transform.position, target.GetInstanceID());

    [HideFromIl2Cpp]
    private Vector2[] GetPath(Vector2 start, Vector2 target, string identifier, bool removeCloseNodes = true)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("Identifier cannot be null or empty");

        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out Vector2[] path, out _, removeCloseNodes)) return Array.Empty<Vector2>();

        if (path.Length == 0) return Array.Empty<Vector2>();

        return path;
    }

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Vector2 target, string identifier, bool removeCloseNodes = true) => GetPath(PlayerControl.LocalPlayer.GetTruePosition(), target, identifier, removeCloseNodes);

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Vector2 target, int identifier, bool removeCloseNodes = true) => GetPath(target, identifier.ToString(), removeCloseNodes);

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Component target, bool removeCloseNodes = true) => GetPath(target.transform.position, target.GetInstanceID(), removeCloseNodes);

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<PathfindingHandler>();
    }
}
