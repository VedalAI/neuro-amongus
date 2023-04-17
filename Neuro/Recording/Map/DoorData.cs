using Neuro.Recording.Common;

namespace Neuro.Recording.Map;

public partial class DoorData
{
    public static DoorData Create(PlainDoor door)
    {
        return new DoorData
        {
            Position = PositionData.Create(door, door),
            IsOpen = door.Open
        };
    }
}
