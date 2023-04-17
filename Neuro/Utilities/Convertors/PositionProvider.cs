using UnityEngine;

namespace Neuro.Utilities.Convertors;

// TODO: Get rid of this class
public struct PositionProvider
{
    public Vector2 Position { get; }

    public PositionProvider(Vector2 position)
    {
        Position = position;
    }

    public static implicit operator PositionProvider(Vector2 position)
    {
        return new PositionProvider(position);
    }

    public static implicit operator PositionProvider(Vector3 position)
    {
        return new PositionProvider(position);
    }

    public static implicit operator PositionProvider(Component component)
    {
        if (!component) return default;

        return new PositionProvider(component.transform.position);
    }

    public static implicit operator PositionProvider(PlayerControl playerControl)
    {
        if (!playerControl) return default;

        return new PositionProvider(playerControl.GetTruePosition());
    }

    public static implicit operator PositionProvider(DeadBody deadBody)
    {
        if (!deadBody) return default;

        return new PositionProvider(deadBody.TruePosition);
    }

    public static implicit operator Vector2(PositionProvider provider) => provider.Position;
}
