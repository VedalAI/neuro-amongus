using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

    [System.Serializable]
    public class Frame
    {
        // Inputs
        public bool isImposter { get; }

        public float killCooldown { get; }

        public MyVector2 directionToNearestTask { get; }
        public bool isEmergencyTask { get; }

        public MyVector2 directionToNearestVent { get; }

        public MyVector2 directionToNearestBody { get; }

        public bool canReport { get; }

        public List<PlayerRecord> playerRecords { get; }

        // Outputs
        public MyVector2 direction { get; }

        public bool report { get; }

        public bool vent { get; }

        public bool kill { get; }

        public bool sabotage { get; }

        public bool doors { get; }

        public Frame(bool isImposter, float killCooldown, MyVector2 directionToNearestTask, bool isEmergencyTask, MyVector2 directionToNearestVent, MyVector2 directionToNearestBody, bool canReport, List<PlayerRecord> playerRecords, MyVector2 direction, bool report, bool vent, bool kill, bool sabotage, bool doors)
        {
            this.isImposter = isImposter;
            this.killCooldown = killCooldown;
            this.directionToNearestTask = directionToNearestTask;
            this.isEmergencyTask = isEmergencyTask;
            this.directionToNearestVent = directionToNearestVent;
            this.directionToNearestBody = directionToNearestBody;
            this.canReport = canReport;
            this.playerRecords = playerRecords;
            this.direction = direction;
            this.report = report;
            this.vent = vent;
            this.kill = kill;
            this.sabotage = sabotage;
            this.doors = doors;
        }
    }

    [System.Serializable]
    public struct PlayerRecord
    {
        public MyVector2 relativeDirection { get; }
        public float distance { get; }

        public PlayerRecord(MyVector2 relativeDirection, float distance)
        {
            this.relativeDirection = relativeDirection;
            this.distance = distance;
        }
    }

    [System.Serializable]
    public record struct MyVector2(float x, float y)
    {
        public static implicit operator MyVector2(Vector2 v) => new MyVector2(v.x, v.y);
    }
}
