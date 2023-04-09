using System.Linq;

namespace Neuro.Communication.AmongUsAI.DataStructures;

public readonly struct VentData
{
    public PositionData Position { get; init; }
    public PositionData[] ConnectingVents { get; init; }

    public VentData(PositionData position, PositionData[] connectingVents)
    {
        Position = position;
        ConnectingVents = connectingVents;
    }

    public static readonly VentData Absent = new();

    public static VentData Create(Vent vent)
    {
        return new VentData
        {
            Position = PositionData.Create(vent, vent),
            ConnectingVents = vent.NearbyVents.Select(v => PositionData.Create(v, v)).ToArray()
        };
    }
}
