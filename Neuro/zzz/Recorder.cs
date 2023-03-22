using System;
using System.Collections.Generic;
using UnityEngine;

namespace Neuro
{
    public class Recorder
    {
        public List<Frame> Frames { get; set; }

        public Recorder()
        {
            Frames = new List<Frame>();
        }
    }

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

    [Serializable]
    public record struct PlayerRecord(MyVector2 relativeDirection, float distance);

    [Serializable]
    public record struct MyVector2(float x, float y)
    {
        public static implicit operator MyVector2(Vector2 v) => new MyVector2(v.x, v.y);
    }
}
