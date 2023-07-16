using System;
using System.Diagnostics;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Neuro.Communication.AmongUsAI;
using Neuro.Pathfinding;

namespace Neuro.Movement;

[RegisterInIl2Cpp, FullShipStatusComponent]
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
            Destroy(this);
            return;
        }

        Instance = this;

        CreateArrow();
    }

    private Vector2 _lastPosition;
    private float _timeSinceLastPositionUpdate;
    private float _unstuckTimer;

    private float _waitTimer;

    public void Wait(float time)
    {
        _waitTimer = time;
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer || !CommunicationHandler.IsPresentAndConnected) return;

        Vector2 truePos = PlayerControl.LocalPlayer.GetTruePosition();
        if (_lastPosition == truePos)
        {
            _timeSinceLastPositionUpdate += Time.fixedDeltaTime;
        }
        else
        {
            _lastPosition = truePos;
            _timeSinceLastPositionUpdate = 0;
        }

        if (_timeSinceLastPositionUpdate > 1)
        {
            // Player is stuck
            _unstuckTimer = 1;
            _timeSinceLastPositionUpdate = 0;
        }
    }

    [Conditional("FULL")]
    public void GetForcedMoveDirection(ref Vector2 direction)
    {
        if (direction != Vector2.zero) return;

        // TODO: We need to adjust this based on player speed setting // TODO: It seems like this already what's happening, but the player still is faster(?)
        direction = ForcedMoveDirection.normalized;

        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.fixedDeltaTime;
            _unstuckTimer = 0f;
            direction = Vector2.zero;
            return;
        }

        // TODO: Fix this (sometimes it triggers when moving back and forth even though the agent isn't actually stuck)
        if (_unstuckTimer > 0f)
        {
            _unstuckTimer -= Time.fixedDeltaTime;
            // pathfind to nearest task

            Info($"Stuck, C# taking over! {_unstuckTimer:F2}s remaining");

            Console closestConsole = null;
            float closestDistance = 999f;

            foreach (Console console in ConsoleFinder.GetOpenableConsoles(true))
            {
                float distance = PathfindingHandler.Instance.GetPathLength(console);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestConsole = console;
                }
            }

            Info(closestConsole != null ? $"Closest console: {closestConsole.name} ({closestDistance:F2})" : "No console found");

            if (closestConsole != null)
            {
                Vector2[] path = PathfindingHandler.Instance.GetPath(closestConsole, false);
                if (path is {Length: > 1})
                {
                    Vector2 normalizedDirection = (path[1] - PlayerControl.LocalPlayer.GetTruePosition()).normalized;

                    // Quantize direction into 8 directions
                    direction = new Vector2(
                        normalizedDirection.x switch
                        {
                            > 0.5f => 1f,
                            < -0.5f => -1f,
                            _ => 0f
                        }, normalizedDirection.y switch
                        {
                            > 0.5f => 1f,
                            < -0.5f => -1f,
                            _ => 0f
                        }).normalized;
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
