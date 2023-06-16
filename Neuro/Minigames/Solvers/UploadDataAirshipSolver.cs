using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Neuro.Cursor;
using Neuro.Gizmos;
using Neuro.Utilities;
using Reactor.Utilities.Extensions;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipUploadGame), false)]
[MinigameOpener(typeof(AutoMultistageMinigame))]
public sealed class UploadDataAirshipSolver : IMinigameSolver<AirshipUploadGame, NormalPlayerTask>, IMinigameOpener
{
    public bool ShouldOpenConsole(Console console, PlayerTask task)
    {
        return task.TaskType == TaskTypes.UploadData;
    }

    public IEnumerator CompleteMinigame(AirshipUploadGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Phone);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        Grid grid = new(minigame);

        Vector2 closest = new(10000, 10000);
        foreach (Grid.Node node in grid._nodes)
        {
            if (Vector2.Distance(minigame.Hotspot.transform.position, node.WorldPosition) <
                Vector2.Distance(minigame.Hotspot.transform.position, closest)) closest = node.WorldPosition;
        }

        minigame.Hotspot.transform.position = minigame.Hotspot.transform.position with {x = closest.x, y = closest.y};

        yield return new WaitForSeconds(0.1f);

        while (!minigame.Poor.IsTouching(minigame.Hotspot))
        {
            if (grid.Cleared)
            {
                yield return CompleteMinigame(minigame, task);
                yield break;
            }

            yield return MoveToUpgradingSignal(minigame, grid, minigame.Poor, true);
        }

        grid.Reset(n => minigame.Poor.OverlapPoint(n.WorldPosition), Color.green);

        while (!minigame.Good.IsTouching(minigame.Hotspot))
        {
            if (grid.Cleared)
            {
                yield return CompleteMinigame(minigame, task);
                yield break;
            }

            yield return MoveToUpgradingSignal(minigame, grid, minigame.Good, false);
        }

        grid.Reset(n => minigame.Good.OverlapPoint(n.WorldPosition), Color.yellow);

        while (!minigame.Perfect.IsTouching(minigame.Hotspot))
        {
            if (grid.Cleared)
            {
                yield return CompleteMinigame(minigame, task);
                yield break;
            }

            yield return MoveToUpgradingSignal(minigame, grid, minigame.Perfect, false);
        }

        InGameCursor.Instance.StopHoldingLMB();
    }

    private static IEnumerator MoveToUpgradingSignal(AirshipUploadGame minigame, Grid grid, Collider2D targetCollider, bool isPoor)
    {
        Grid.Node targetNode = grid.GetNextTarget(false && !isPoor);
        Vector2 phoneStartPosition = minigame.Phone.transform.position;

        float distance = Vector2.Distance(phoneStartPosition, targetNode.WorldPosition);
        float time = distance / 6f;

        for (float t = 0; t < time; t += Time.deltaTime)
        {
            if (!targetNode.Available) break;
            if (targetCollider.IsTouching(minigame.Hotspot)) break;

            InGameCursor.Instance.SnapTo(Vector2.Lerp(phoneStartPosition, targetNode.WorldPosition, t / time));
            grid.DisableWhere(p => !targetCollider.IsTouching(minigame.Hotspot) && targetCollider.OverlapPoint(p.WorldPosition));

            yield return null;
        }

        yield return null;
    }

    private sealed class Grid
    {
        private const int SCREEN_NODE_DISTANCE = 25;

        internal readonly HashSet<Node> _nodes;
        private readonly HashSet<Node> _availableNodesCache = new();

        public Grid(AirshipUploadGame minigame)
        {
            _nodes = new HashSet<Node>();

            GameObject visualPointParentObj = new("Visual Point Parent")
            {
                transform =
                {
                    parent = minigame.transform,
                },
                layer = LayerMask.NameToLayer("UI")
            };
            Transform visualPointParent = visualPointParentObj.transform;
            visualPointParentObj.SetActive(GizmosDebugTab.EnableAirshipUploadNodes);

            for (int x = 0; x < Screen.width; x += SCREEN_NODE_DISTANCE)
            {
                for (int y = 0; y < Screen.height; y += SCREEN_NODE_DISTANCE)
                {
                    _nodes.Add(new Node(new Vector2Int(x, y), visualPointParent));
                }
            }
        }

        public bool Cleared => _nodes.Count == 0;

        public void Reset(Func<Node, bool> predicate, Color resetColor)
        {
            foreach (Node node in _nodes)
            {
                if (predicate(node))
                {
                    if (node.Available) node.Enable(resetColor);
                }
                else
                {
                    node.Disable();
                }
            }
        }

        public Node GetNextTarget(bool checkNeighbors)
        {
            if (Cleared) return null;

            RefreshAvailableNodesCache();

            return !checkNeighbors
                ? _nodes.Where(n => n.Available).Random()
                : _nodes.OrderByDescending(n => CountActiveNeighbors(n, 5)).First();
        }

        public void DisableWhere(Func<Node, bool> predicate)
        {
            foreach (Node node in _nodes.Where(predicate))
            {
                node.Disable();
            }
        }

        private void RefreshAvailableNodesCache()
        {
            _availableNodesCache.Clear();
            _availableNodesCache.UnionWith(_nodes.Where(n => n.Available));
        }

        private int CountActiveNeighbors(Node node, int range)
        {
            return _availableNodesCache.Count(n => Math.Max(
                Math.Abs(node.ScreenPosition.x - n.ScreenPosition.x) / SCREEN_NODE_DISTANCE,
                Math.Abs(node.ScreenPosition.y - n.ScreenPosition.y) / SCREEN_NODE_DISTANCE) <= range);
        }

        public class Node
        {
            private static readonly Color _disabledColor = Color.gray;

            public Vector2Int ScreenPosition { get; }
            public Vector2 WorldPosition { get; }
            public bool Available { get; private set; } = true;

            private LineRenderer visual;

            public Node(Vector2Int screenPosition, Transform visualPointParent)
            {
                ScreenPosition = screenPosition;
                WorldPosition = NeuroUtilities.MainCamera.ScreenToWorldPoint(new Vector2(screenPosition.x, screenPosition.y));
                CreateVisual(visualPointParent);
            }

            public void Disable()
            {
                Available = false;
                visual.startColor = visual.endColor = _disabledColor;
            }

            public void Enable(Color color)
            {
                Available = true;
                visual.startColor = visual.endColor = color;
            }

            private void CreateVisual(Transform parent)
            {
                GameObject nodeVisualPoint = new("Gizmo (Visual Point)")
                {
                    transform =
                    {
                        parent = parent,
                        position = WorldPosition
                    },
                    layer = LayerMask.NameToLayer("UI")
                };
                nodeVisualPoint.transform.localPosition = nodeVisualPoint.transform.localPosition with {z = -200};
                LineRenderer renderer = nodeVisualPoint.AddComponent<LineRenderer>();
                renderer.SetPosition(0, nodeVisualPoint.transform.position);
                renderer.SetPosition(1, nodeVisualPoint.transform.position + new Vector3(0, 0.1f));
                renderer.widthMultiplier = 0.1f;
                renderer.positionCount = 2;
                renderer.material = NeuroUtilities.MaskShaderMat;
                renderer.startColor = Color.red;
                renderer.endColor = Color.red;

                visual = renderer;
            }
        }
    }
}
