using System;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Movement;

[RegisterInIl2Cpp]
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

    public void GetForcedMoveDirection(ref Vector2 direction)
    {
        if (direction != Vector2.zero) return;
        direction = ForcedMoveDirection;

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

    [EventHandler(EventTypes.GameStarted)]
    public static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<MovementHandler>();
    }
}
