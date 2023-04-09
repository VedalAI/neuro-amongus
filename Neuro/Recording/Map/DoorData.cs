using System.IO;
using Neuro.Communication.AmongUsAI;

namespace Neuro.Recording.Map;

public readonly struct DoorData : ISerializable
{
    private PositionData Position { get; init; } = default;
    private bool IsOpen { get; init; } = true;

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

    public static DoorData Create(PlainDoor door)
    {
        return new DoorData
        {
            Position = PositionData.Create(door, door),
            IsOpen = door.Open
        };
    }
}
