using Neuro.DependencyInjection;
using UnityEngine;

namespace Neuro.Movement;

public interface IMovementHandler : IContextAccepter
{
    public Vector2? GetForcedMoveDirection(Vector2 actualDirection);

    public Vector2 LastMoveDirection { get; }

    public Vector2 DirectionToNearestTask { get; }
}
