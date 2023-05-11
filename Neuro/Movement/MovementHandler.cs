using System;
using System.Collections.Generic;
using System.Diagnostics;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Neuro.Communication.AmongUsAI;
using Neuro.Pathfinding;

namespace Neuro.Movement;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class MovementHandler : MonoBehaviour
{
    public static MovementHandler Instance { get; private set; }

    public MovementHandler(IntPtr ptr) : base(ptr)
    {
    }

    public Vector2 ForcedMoveDirection { get; set; }

    private LineRenderer _arrow;

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;

        CreateArrow();
    }

    private readonly Queue<Vector2> _positionHistory = new();
    private float _unstuckTimer = 0f;

    [Conditional("FULL")]
    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer || CommunicationHandler.Instance == null || !CommunicationHandler.Instance.IsConnected) return;

        if (_positionHistory.Count > 100) _positionHistory.Dequeue();
        _positionHistory.Enqueue(PlayerControl.LocalPlayer.GetTruePosition());
        if (_positionHistory.Count < 50) return;

        // Get the average position of the last 100 frames
        Vector2 averagePosition = Vector2.zero;
        foreach (Vector2 position in _positionHistory) averagePosition += position;
        averagePosition /= _positionHistory.Count;

        if ((averagePosition - PlayerControl.LocalPlayer.GetTruePosition()).magnitude < 0.1f)
        {
            // player is stuck
            _unstuckTimer = 2f;
        }
    }

    public void GetForcedMoveDirection(ref Vector2 direction)
    {
        if (direction != Vector2.zero) return;

        // TODO: We need to adjust this based on player speed setting // TODO: It seems like this already what's happening, but the player still is faster(?)
        direction = ForcedMoveDirection.normalized;

        if (_unstuckTimer > 0f)
        {
            _unstuckTimer -= Time.fixedDeltaTime;
            // pathfind to nearest task

            Info($"Stuck, C# taking over! {_unstuckTimer:F2}s remaining");

            Console closestConsole = null;
            float closestDistance = 999f;

            foreach (Console console in NeuroUtilities.GetOpenableConsoles(true))
            {
                float distance = PathfindingHandler.Instance.GetPathLength(console);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestConsole = console;
                }
            }

            if (closestConsole != null)
            {
                Vector2[] path = PathfindingHandler.Instance.GetPath(closestConsole, false);
                if (path is {Length: > 0})
                {
                    direction = (path[0] - PlayerControl.LocalPlayer.GetTruePosition()).normalized;
                }
            }
        }

        LineRenderer renderer = _arrow;
        renderer.SetPosition(0, PlayerControl.LocalPlayer.GetTruePosition());
        renderer.SetPosition(1, PlayerControl.LocalPlayer.GetTruePosition() + direction);
    }

    private void CreateArrow()
    {
        GameObject arrowObject = new("Arrow");
        arrowObject.transform.parent = transform;
        _arrow = arrowObject.AddComponent<LineRenderer>();

        _arrow.startWidth = 0.4f;
        _arrow.endWidth = 0.05f;
        _arrow.positionCount = 2;
        _arrow.material = new Material(Shader.Find("Sprites/Default"));
        _arrow.startColor = Color.blue;
        _arrow.endColor = Color.cyan;
    }
}
