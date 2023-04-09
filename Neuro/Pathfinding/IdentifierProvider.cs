using System;

namespace Neuro.Pathfinding;

public readonly struct IdentifierProvider
{
    public string Identifier { get; }

    public IdentifierProvider(string identifier)
    {
        Identifier = identifier;
    }

    public static implicit operator IdentifierProvider(PlainDoor door) => new($"PlainDoor_{ShipStatus.Instance.AllDoors.IndexOf(new Func<PlainDoor, bool>(d => d.GetInstanceID() == door.GetInstanceID()))}");

    public static implicit operator string(IdentifierProvider provider) => provider.Identifier;
    public override string ToString() => this;
}
