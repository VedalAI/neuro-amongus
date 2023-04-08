using UnityEngine;

namespace Neuro.Utilities;

public struct PositionProvider
{
    public Vector2 Position { get; }

    public PositionProvider(Vector2 position)
    {
        Position = position;
    }

    public static implicit operator PositionProvider(Vector2 position) => new(position);
    public static implicit operator PositionProvider(Component component) => new(component.transform.position);
    public static implicit operator PositionProvider(PlayerControl playerControl) => new(playerControl.GetTruePosition());
    public static implicit operator PositionProvider(DeadBody deadBody) => new(deadBody.TruePosition);

    public static implicit operator Vector2(PositionProvider provider) => provider.Position;
}
