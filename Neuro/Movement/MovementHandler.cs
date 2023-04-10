using Neuro.Tasks;
using UnityEngine;

namespace Neuro.Movement;

public sealed class MovementHandler
{
    public Vector2 LastMoveDirection { get; private set; }
    public Vector2 DirectionToNearestTask { get; private set; }

    public Vector2? forceMovementDirection { get; set; }

    public LineRenderer Arrow { get; set; }

    // TODO: This method wasn't actually changing anything, why not?
    public void GetForcedMoveDirection(ref Vector2 direction)
    {
        // TODO: This should be changed as MovementHandler should not assign to fields in different handlers
        TasksHandler handler = NeuroPlugin.Instance.Tasks;

        LastMoveDirection = direction;
        if (handler.CurrentPath.Length <= 0 || handler.PathIndex == -1)
        {
            DirectionToNearestTask = Vector2.zero;
            return;
        }

        Vector2 nextWaypoint = handler.CurrentPath[handler.PathIndex];

        while (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), nextWaypoint) < 1f)
        {
            handler.PathIndex++;
            if (handler.PathIndex > handler.CurrentPath.Length - 1)
            {
                handler.PathIndex = handler.CurrentPath.Length - 1;
                nextWaypoint = handler.CurrentPath[handler.PathIndex];
                break;
            }

            nextWaypoint = handler.CurrentPath[handler.PathIndex];
        }

        DirectionToNearestTask = (nextWaypoint - PlayerControl.LocalPlayer.GetTruePosition()).normalized;

        LineRenderer renderer = Arrow;
        renderer.SetPosition(0, PlayerControl.LocalPlayer.GetTruePosition());
        renderer.SetPosition(1, PlayerControl.LocalPlayer.GetTruePosition() + DirectionToNearestTask);
    }
}
