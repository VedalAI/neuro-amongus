using Il2CppInterop.Runtime.Attributes;
using System;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace Neuro.Minigames;

[RegisterInIl2Cpp, FullShipStatusComponent]
public sealed class MinigameTimeHandler : MonoBehaviour
{
    public static MinigameTimeHandler Instance { get; private set; }

    public record MinigameTimeKey(TaskTypes Type, int Step, NormalPlayerTask.TimerState TimerState);

    [HideFromIl2Cpp] public Dictionary<MinigameTimeKey, List<float>> MinigameTimes { get; } = new Dictionary<MinigameTimeKey, List<float>>();

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
            MinigameTimes[_minigameKey] = new List<float>() {time};
        }
    }

    public void Clear()
    {
        MinigameTimes.Clear();
    }

    [HideFromIl2Cpp]
    public void StartTimer(Minigame minigame, Func<bool> hideCondition)
    {
        if (!minigame.MyTask) return;
        if (PlayerTask.TaskIsEmergency(minigame.MyTask)) return;

        _minigameKey = new MinigameTimeKey(minigame.MyNormTask.TaskType, minigame.MyNormTask.TaskStep, minigame.MyNormTask.TimerStarted);
        _startTime = Time.time;
        _stopTimerCondition = hideCondition;
    }

    private void Stop()
    {
        _stopTimerCondition = null;
        AddMinigameTime(Time.time - _startTime);
    }
}