using System.IO;
using Neuro.Communication.AmongUsAI;

namespace Neuro.Map.DataStructures;

public readonly struct DoorData : ISerializable
{
    public PositionData Position { get; init; } = PositionData.Absent;
    public bool IsOpen { get; init; } = true;

    public DoorData(PositionData position, bool isOpen)
    {
        Position = position;
        IsOpen = isOpen;
    }

    public void Serialize(BinaryWriter writer)
    {
        Position.Serialize(writer);
        writer.Write(IsOpen);
    }

    public static readonly DoorData Absent = new();

    public static DoorData Create(PlainDoor door)
    {
        return new DoorData
        {
            Position = PositionData.Create(door, door),
            IsOpen = door.Open
        };
    }
}
