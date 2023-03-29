using System.Collections.Generic;

namespace Neuro.Recording.DataStructures;

public record Frame(
    bool IsImposter,
    float KillCooldown,
    MyVector2 DirectionToNearestTask,
    bool IsEmergencyTask,
    MyVector2 DirectionToNearestVent,
    List<Vent> NearbyVents,
    MyVector2 DirectionToNearestBody,
    bool CanReport,
    Dictionary<byte, PlayerRecord> PlayerRecords,
    MyVector2 Direction,
    bool Report,
    bool Vent,
    bool InVent,
    bool Kill,
    bool Sabotage,
    SabotageTypes SabotageUsed,
    bool Doors,
    List<PlainDoor> NearbyDoors,
    List<PlainDoor> DoorsUsed
);
