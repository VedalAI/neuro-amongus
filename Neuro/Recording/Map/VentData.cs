using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neuro.Communication.AmongUsAI;

namespace Neuro.Recording.Map;

public readonly struct VentData : ISerializable
{
    public PositionData Position { get; init; }
    public List<PositionData> ConnectingVents { get; init; }

    public VentData(PositionData position, List<PositionData> connectingVents)
    {
        Position = position;
        ConnectingVents = connectingVents;
    }

    public void Serialize(BinaryWriter writer)
    {
        Position.Serialize(writer);
        writer.Write((byte) ConnectingVents.Count);
        foreach (PositionData vent in ConnectingVents)
            vent.Serialize(writer);
    }

    public static VentData Create(Vent vent)
    {
        return new VentData
        {
            Position = PositionData.Create(vent, vent),
            ConnectingVents = vent.NearbyVents.Select(v => PositionData.Create(v, v)).ToList()
        };
    }
}
