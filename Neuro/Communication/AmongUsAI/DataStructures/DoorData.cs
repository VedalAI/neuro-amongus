namespace Neuro.Communication.AmongUsAI.DataStructures;

public readonly struct DoorData
{
    public PositionData Position { get; init; } = PositionData.Absent;
    public bool IsOpen { get; init; } = true;

    public DoorData(PositionData position, bool isOpen)
    {
        Position = position;
        IsOpen = isOpen;
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
