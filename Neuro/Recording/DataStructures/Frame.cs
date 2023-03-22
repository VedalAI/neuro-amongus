using System;
using System.Collections.Generic;

namespace Neuro.Recording.DataStructures;

[Serializable]
public record Frame(bool isImposter,
    float killCooldown,
    MyVector2 directionToNearestTask,
    bool isEmergencyTask,
    MyVector2 directionToNearestVent,
    MyVector2 directionToNearestBody,
    bool canReport,
    List<PlayerRecord> playerRecords,
    MyVector2 direction,
    bool report,
    bool vent,
    bool kill,
    bool sabotage,
    bool doors
);
