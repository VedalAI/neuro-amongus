using Neuro.DependencyInjection;
using Neuro.Tasks;
using UnityEngine;

namespace Neuro.Movement;

public class MovementHandler : IMovementHandler
{
    public IContextProvider Context { get; set; }

    public Vector2 LastMoveDirection { get; private set; }
    public Vector2 DirectionToNearestTask { get; private set; }

    public LineRenderer Arrow { get; set; }

    public Vector2? GetForcedMoveDirection(Vector2 actualDirection)
    {
        // TODO: This is terrible, MovementHandler should not assign to fields in different handlers
        ITasksHandler handler = Context.TasksHandler;

        LastMoveDirection = actualDirection;
        if (handler.CurrentPath.Length > 0 && handler.PathIndex != -1)
        {
            Vector2 nextWaypoint = handler.CurrentPath[handler.PathIndex];

            while (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), nextWaypoint) < 0.75f)
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
            renderer.widthMultiplier = 0.1f;
            renderer.positionCount = 2;
            renderer.startColor = Color.red;
        }
        else
        {
            DirectionToNearestTask = Vector2.zero;
        }

        return null;
    }
}
