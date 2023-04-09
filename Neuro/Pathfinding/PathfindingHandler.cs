using System;
using System.Linq;
using Neuro.Communication.AmongUsAI.DataStructures;
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

    public PathfindingHandler(IntPtr ptr) : base(ptr)
    {
    }

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
        EventManager.RegisterHandler(this);

        _thread = new PathfindingThread(GenerateNodeGrid(), ShipStatus.Instance.MeetingSpawnCenter + Vector2.down * ShipStatus.Instance.SpawnRadius);
        _thread.Start();
    }

    private void OnDestroy()
    {
        Warning("Running Stop");
        _thread.Stop();
    }

    private Node[,] GenerateNodeGrid()
    {
        Node[,] grid = new Node[GRID_SIZE, GRID_SIZE];

        const float NODE_RADIUS = 1 / GRID_DENSITY;

        for (int x = GRID_LOWER_BOUNDS; x < GRID_UPPER_BOUNDS; x++)
        for (int y = GRID_LOWER_BOUNDS; y < GRID_UPPER_BOUNDS; y++)
        {
            Vector2 point = new(x / GRID_DENSITY, y / GRID_DENSITY);

            Collider2D[] cols = Physics2D.OverlapCircleAll(point, NODE_RADIUS, LayerMask.GetMask("Ship", "ShortObjects"));
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

    public float CalculateTotalDistance(PositionProvider start, PositionProvider target, IdentifierProvider identifier)
    {
        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out MyVector2[] path)) return -1;

        if (path.Length == 0) return -1;

        float distance = 0f;
        for (int i = 0; i < path.Length - 1; i++)
        {
            distance += Vector2.Distance(path[i], path[i + 1]);
        }

        return distance;
    }

    public Vector2 CalculateOffsetToFirstNode(PositionProvider start, PositionProvider target, IdentifierProvider identifier)
    {
        _thread.RequestPath(start, target, identifier);
        if (!_thread.TryGetPath(identifier, out MyVector2[] path)) return Vector2.zero;

        if (path.Length == 0) return Vector2.zero;

        Vector2 firstNode = path[0];

        return firstNode - start;
    }

    [EventHandler(EventTypes.GameStarted)]
    public static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<PathfindingHandler>();
    }
}
