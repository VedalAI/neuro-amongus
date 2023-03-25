using System.Collections.Generic;

namespace Neuro.Recording.DataStructures;

public record Frame(
    bool IsImposter,
    float KillCooldown,
    MyVector2 DirectionToNearestTask,
    bool IsEmergencyTask,
    MyVector2 DirectionToNearestVent,
    MyVector2 DirectionToNearestBody,
    bool CanReport,
    List<PlayerRecord> PlayerRecords,
    MyVector2 Direction,
    bool Report,
    bool Vent,
    bool Kill,
    bool Sabotage,
    bool Doors
);
