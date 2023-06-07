using Il2CppInterop.Runtime.Attributes;
using System;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Neuro.Minigames;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class MinigameTimeHandler : MonoBehaviour
{
    public static MinigameTimeHandler Instance { get; private set; }

    public class MinigameTimeKey : IEquatable<MinigameTimeKey>
    {
        public TaskTypes Type { get; private set; }
        public int Step { get; private set; }

        public MinigameTimeKey(TaskTypes type, int step)
        {
            Type = type;
            Step = step;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as MinigameTimeKey);
        }

        public bool Equals(MinigameTimeKey obj)
        {
            return obj != null && obj.Type == Type && obj.Step == Step;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() + Step.GetHashCode();
        }
    }

    [HideFromIl2Cpp]
    public Dictionary<MinigameTimeKey, List<float>> MinigameTimes { get; } = new Dictionary<MinigameTimeKey, List<float>>();

    private MinigameTimeKey _minigameKey;
    private float _startTime;
    private Func<bool> _stopTimerCondition;

    public MinigameTimeHandler(IntPtr ptr) : base(ptr)
    {
    }

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        if (_stopTimerCondition != null && _stopTimerCondition())
        {
            Stop();
        }
    }

    private void AddMinigameTime(float time)
    {
        if (MinigameTimes.TryGetValue(_minigameKey, out List<float> list))
        {
            list.Add(time);
        }
        else
        {
            MinigameTimes[_minigameKey] = new List<float>() { time };
        }
        _minigameKey = null;
    }

    public void Clear()
    {
        MinigameTimes.Clear();
    }

    public void StartTimer(Minigame minigame, Func<bool> hideCondition)
    {
        if (minigame.MyTask)
        {
            _minigameKey = new MinigameTimeKey(minigame.MyTask.TaskType, minigame.MyTask.TaskStep);
            _startTime = Time.time;
            _stopTimerCondition = hideCondition;
        }
    }

    private void Stop()
    {
        _stopTimerCondition = null;
        if (_minigameKey != null)
        {
            AddMinigameTime(Time.time - _startTime);
        }
    }
}
