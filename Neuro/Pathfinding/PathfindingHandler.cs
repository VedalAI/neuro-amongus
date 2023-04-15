using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Pathfinding.DataStructures;
using Neuro.Utilities;
using Neuro.Utilities.Convertors;
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

        const float NODE_RADIUS = 1 / GRID_DENSITY;

        for (int x = GRID_LOWER_BOUNDS; x < GRID_UPPER_BOUNDS; x++)
        for (int y = GRID_LOWER_BOUNDS; y < GRID_UPPER_BOUNDS; y++)
        {
            Vector2 point = new(x / GRID_DENSITY, y / GRID_DENSITY);

            Collider2D[] cols = Physics2D.OverlapCircleAll(point, NODE_RADIUS, Constants.ShipAndAllObjectsMask);
            int validColsCount = cols.Count(col =>
                !col.isTrigger &&
                !col.GetComponentInParent<Vent>() &&
                !col.GetComponentInParent<SomeKindaDoor>()
            );

            // TODO: Add edge case for Airship ladders

            bool accessible = validColsCount == 0;
            grid[x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS] = new Node(accessible, point, x + GRID_UPPER_BOUNDS, y + GRID_UPPER_BOUNDS);
        }

        return grid;
    }

    public float GetPathLength(PositionProvider start, PositionProvider target, IdentifierProvider identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("Identifier cannot be null or empty");

        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out _, out float length)) return -1;

        return length;
    }

    public Vector2 GetFirstNodeInPath(PositionProvider start, PositionProvider target, IdentifierProvider identifier)
    {
        if (string.IsNullOrEmpty(identifier)) throw new ArgumentException("Identifier cannot be null or empty");

        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out Vector2[] path, out _)) return Vector2.zero;

        if (path.Length == 0) return Vector2.zero;

        return path[0];
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<PathfindingHandler>();
    }
}
