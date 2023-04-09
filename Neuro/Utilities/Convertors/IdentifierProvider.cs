using System;

namespace Neuro.Utilities.Convertors;

public readonly struct IdentifierProvider
{
    public string Identifier { get; }

    public IdentifierProvider(string identifier)
    {
        Identifier = identifier;
    }

    public static implicit operator IdentifierProvider(PlainDoor door)
    {
        if (!ShipStatus.Instance) return default;

        int index = ShipStatus.Instance.AllDoors.IndexOf(new Func<PlainDoor, bool>(d => d.GetInstanceID() == door.GetInstanceID()));
        if (index == -1) return default;

        return new IdentifierProvider($"PlainDoor_{index}");
    }

    public static implicit operator IdentifierProvider(Vent vent)
    {
        if (!vent) return default;

        return new IdentifierProvider($"Vent_{vent.Id}");
    }

    public static implicit operator string(IdentifierProvider provider) => provider.Identifier;
    public override string ToString() => this;
}
