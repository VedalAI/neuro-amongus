using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Pathfinding.DataStructures;
using Neuro.Recording.Common;
using Reactor.Utilities.Attributes;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

namespace Neuro.Pathfinding;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class PathfindingHandler : MonoBehaviour
{
    public float GridDensity { get; set; }
    public int GridBaseWidth { get; set; }

    public int GridSize => (int) (GridBaseWidth * GridDensity);
    public int GridLowerBounds => Mathf.RoundToInt(GridSize / -2f);
    public int GridUpperBounds => Mathf.RoundToInt(GridSize / 2f);

    public GameObject VisualPointParent { get; private set; }

    public static PathfindingHandler Instance { get; private set; }

    private static Vector2 _localPlayerPosition => (Vector2) PlayerControl.LocalPlayer.transform.position + PlayerControl.LocalPlayer.Collider.offset / 2;

    public PathfindingHandler(IntPtr ptr) : base(ptr)
    {
    }

    private PathfindingThread _thread;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        InitializeGridSizes();
        InitializeThread();
    }

    private void OnDestroy()
    {
        System.Console.WriteLine("[PATHFINDING] Trying to shut down (destroyed)...");
        _thread.Stop();
    }

    private void OnApplicationQuit()
    {
        System.Console.WriteLine("[PATHFINDING] Trying to shut down...");
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
        if (VisualPointParent) VisualPointParent.Destroy();
        VisualPointParent = new GameObject("Visual Point Parent");

        List<Vector2> accessiblePositions = new()
        {
            ShipStatus.Instance.MeetingSpawnCenter + Vector2.down * ShipStatus.Instance.SpawnRadius
        };

        if (ShipStatus.Instance.GetTypeForMessage() == MapType.Airship)
        {
            accessiblePositions.Add(ShipStatus.Instance.AllConsoles.First(c => c.TaskTypes.Contains(TaskTypes.EnterIdCode)).transform.position + new Vector3(0, -0.5f));
            accessiblePositions.Add(ShipStatus.Instance.AllConsoles.First(c => c.TaskTypes.Contains(TaskTypes.StopCharles) && c.ConsoleId == 1).transform.position + new Vector3(0, -0.5f));
        }

        _thread?.Stop();
        _thread = new PathfindingThread(GenerateNodeGrid(), accessiblePositions, VisualPointParent.transform);
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
            Component transport = null;
            bool accessible = false;
            for (int i = 0; i < 9; i++)
            {
                int b = (i * 4 + 4) % 9; // Noncontinuous linear index through the array
                if (TryGetAccessiblePoint(x - GridUpperBounds + offsetCoords[b].x, y - GridUpperBounds + offsetCoords[b].y, out point, out transport))
                {
                    accessible = true;
                    break;
                }
            }

            Node node;
            switch (transport)
            {
                case Ladder ladder:
                    node = new Node(point, new Vector2Int(x, y), accessible)
                    {
                        color = Color.green,
                        transportSelfId = ladder.GetInstanceID(),
                        transportTargetId = ladder.Destination.GetInstanceID()
                    };
                    break;

                case PlatformConsole platform:
                    node = new PlatformNode(point, new Vector2Int(x, y), accessible, platform)
                    {
                        color = Color.blue,
                        transportSelfId = platform.GetInstanceID(),
                        transportTargetId = FindObjectsOfType<PlatformConsole>().First(p => p.GetInstanceID() != platform.GetInstanceID()).GetInstanceID()
                    };
                    break;

                default:
                    node = new Node(point, new Vector2Int(x, y), accessible);
                    break;
            }

            grid[x, y] = node;
        }

        return grid;
    }

    private bool TryGetAccessiblePoint(float x, float y, out Vector2 point, out Component usable)
    {
        float nodeRadius = 1 / GridDensity;
        point = new Vector2(x / GridDensity, y / GridDensity);

        // Get the colliders overlapping this point
        Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(point, nodeRadius, Constants.ShipAndAllObjectsMask);

        // OUT the ladder that it's overlapping, if any
        usable = null;
        if (overlappingColliders.Select(c => c.GetComponentInParent<Ladder>()).FirstOrDefault(l => l) is { } foundLadder) usable = foundLadder;
        if (overlappingColliders.Select(c => c.GetComponentInParent<PlatformConsole>()).FirstOrDefault(p => p) is { } foundPlatform) usable = foundPlatform;

        // Return whether or not this point is accessible
        return overlappingColliders.All(col => col.isTrigger || col.GetComponentInParent<SomeKindaDoor>());
    }

    private float GetPathLength(Vector2 start, Vector2 target, string identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("Identifier cannot be null or empty");

        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out _, out float length)) return -1;

        return length;
    }

    public float GetPathLength(Vector2 target, string identifier) => GetPathLength(_localPlayerPosition, target, identifier);

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

    public Vector2 GetFirstNodeInPath(Vector2 target, string identifier) => GetFirstNodeInPath(_localPlayerPosition, target, identifier);

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
    public Vector2[] GetPath(Vector2 target, string identifier, bool removeCloseNodes = true) => GetPath(_localPlayerPosition, target, identifier, removeCloseNodes);

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Vector2 target, int identifier, bool removeCloseNodes = true) => GetPath(target, identifier.ToString(), removeCloseNodes);

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Component target, bool removeCloseNodes = true) => GetPath(target.transform.position, target.GetInstanceID(), removeCloseNodes);
}