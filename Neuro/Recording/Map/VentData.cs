using System.IO;
using System.Linq;
using Neuro.Communication.AmongUsAI;

namespace Neuro.Recording.Map;

public readonly struct VentData : ISerializable
{
    public PositionData Position { get; init; }
    public PositionData[] ConnectingVents { get; init; }

    public VentData(PositionData position, PositionData[] connectingVents)
    {
        Position = position;
        ConnectingVents = connectingVents;
    }

    public void Serialize(BinaryWriter writer)
    {
        Position.Serialize(writer);

        for (int i = 0; i < 3; i++)
            ConnectingVents[i].Serialize(writer);
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
