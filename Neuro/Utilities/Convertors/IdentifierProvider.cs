using System;

namespace Neuro.Utilities.Convertors;

// TODO: Ideally this should be removed and replaced with just component.GetInstanceID(), however it helps with giving everything a name in the debug tab
public readonly struct IdentifierProvider
{
    public string Identifier { get; }

    public IdentifierProvider(string identifier)
    {
        Identifier = identifier;
    }

    public static implicit operator IdentifierProvider(Console console)
    {
        if (!console) return default;

        int index = ShipStatus.Instance.AllConsoles.IndexOf(new Func<Console, bool>(c => c.GetInstanceID() == console.GetInstanceID()));
        if (index == -1) return default;

        return new IdentifierProvider($"Console_{index}");
    }

    public static implicit operator IdentifierProvider(PlainDoor door)
    {
        if (!door || !ShipStatus.Instance) return default;

        int index = ShipStatus.Instance.AllDoors.IndexOf(new Func<PlainDoor, bool>(d => d.GetInstanceID() == door.GetInstanceID()));
        if (index == -1) return default;

        return new IdentifierProvider($"PlainDoor_{index}");
    }

    public static implicit operator IdentifierProvider(PlayerControl playerControl)
    {
        if (!playerControl) return default;

        return new IdentifierProvider($"PlayerControl_{playerControl.PlayerId}");
    }

    public static implicit operator IdentifierProvider(Vent vent)
    {
        if (!vent) return default;

        return new IdentifierProvider($"Vent_{vent.Id}");
    }

    public static implicit operator string(IdentifierProvider provider) => provider.Identifier;
    public override string ToString() => this;
}
