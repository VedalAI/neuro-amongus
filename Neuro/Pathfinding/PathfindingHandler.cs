using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Pathfinding.DataStructures;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Pathfinding;

[RegisterInIl2Cpp]
public sealed class PathfindingHandler : MonoBehaviour
{
    public const float GRID_DENSITY = 5f;
    public const int GRID_BASE_WIDTH = 100;

    public const int GRID_SIZE = (int)(GRID_BASE_WIDTH * GRID_DENSITY);
    public const int GRID_LOWER_BOUNDS = GRID_SIZE / -2;
    public const int GRID_UPPER_BOUNDS = GRID_SIZE / 2;

    public static PathfindingHandler Instance { get; private set; }

    public PathfindingHandler(IntPtr ptr) : base(ptr) { }

    private PathfindingThread _thread;

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;

        _thread = new PathfindingThread(GenerateNodeGrid(), ShipStatus.Instance.MeetingSpawnCenter + Vector2.down * ShipStatus.Instance.SpawnRadius);
        _thread.Start();
    }

    private void OnDestroy()
    {
        _thread.Stop();
    }

    [HideFromIl2Cpp]
    private Node[,] GenerateNodeGrid()
    {
        Node[,] grid = new Node[GRID_SIZE, GRID_SIZE];

        const float OFFSET = 1 / 5f; // Must be less than 1 / 4f or it will flood fill through walls
        Vector2[] offsetCoords =
        {
            new(-OFFSET, -OFFSET), new(0, -OFFSET), new(OFFSET, -OFFSET),
            new(-OFFSET, 0), Vector2.zero, new(OFFSET, 0),
            new(-OFFSET, OFFSET), new(0, OFFSET), new(OFFSET, OFFSET)
        };

        for (int x = GRID_LOWER_BOUNDS; x < GRID_UPPER_BOUNDS; x++)
        for (int y = GRID_LOWER_BOUNDS; y < GRID_UPPER_BOUNDS; y++)
        {
            Vector2 point = Vector2.zero;
            bool accessible = false;
            for (int i = 0; i < 9; i++)
            {
                int b = (i * 4 + 4) % 9; // Noncontinuous linear index through the array
                if (TryGetAccessiblePoint(x + offsetCoords[b].x, y + offsetCoords[b].y, out point))
                {
                    accessible = true;
                    break;
                }
            }

            grid[x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS] = new Node(accessible, point, x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS);
        }

        return grid;
    }

    private bool TryGetAccessiblePoint(float x, float y, out Vector2 point)
    {
        const float NODE_RADIUS = 1 / GRID_DENSITY;
        point = new(x / GRID_DENSITY, y / GRID_DENSITY);

        Collider2D[] cols = Physics2D.OverlapCircleAll(point, NODE_RADIUS, Constants.ShipAndAllObjectsMask);
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
    private Vector2[] GetPath(Vector2 start, Vector2 target, string identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("Identifier cannot be null or empty");

        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out Vector2[] path, out _)) return Array.Empty<Vector2>();

        if (path.Length == 0) return Array.Empty<Vector2>();

        return path;
    }

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Vector2 target, string identifier) => GetPath(PlayerControl.LocalPlayer.GetTruePosition(), target, identifier);

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Vector2 target, int identifier) => GetPath(target, identifier.ToString());

    [HideFromIl2Cpp]
    public Vector2[] GetPath(Component target) => GetPath(target.transform.position, target.GetInstanceID());

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<PathfindingHandler>();
    }
}
