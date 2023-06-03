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
    public class MinigameTimeInstance
    {
        public float Time;
        public float CloseTimeDelay;
        public float Total { get => Time + CloseTimeDelay; }

        public MinigameTimeInstance()
        {
            Time = 0.0f;
            CloseTimeDelay = 0.0f;
        }
    }

    public static MinigameTimeHandler Instance { get; private set; }

    [HideFromIl2Cpp]
    public Dictionary<string, List<MinigameTimeInstance>> MinigameTimes { get; } = new Dictionary<string, List<MinigameTimeInstance>>();

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

    public void AddMinigameTime(Minigame minigame, float time)
    {
        string name = minigame.GetIl2CppType().Name;
        if (MinigameTimes.TryGetValue(name, out List<MinigameTimeInstance> list))
        {
            if (list.Last().Time > 0.0f)
            {
                MinigameTimes[name].Add(new MinigameTimeInstance() { Time = time });
            }
            else
            {
                list.Last().Time = time;
            }
        }
        else
        {
            MinigameTimes[name] = new List<MinigameTimeInstance>() { new MinigameTimeInstance() { Time = time } };
        }
    }

    public void AddMinigameCloseTimeDelay(Minigame minigame, float time)
    {
        string name = minigame.GetIl2CppType().Name;
        if (MinigameTimes.TryGetValue(name, out List<MinigameTimeInstance> list))
        {
            if (list.Last().CloseTimeDelay > 0.0f)
            {
                MinigameTimes[name].Add(new MinigameTimeInstance() { CloseTimeDelay = time });
            }
            else
            {
                list.Last().CloseTimeDelay = time;
            }
        }
        else
        {
            MinigameTimes[name] = new List<MinigameTimeInstance>() { new MinigameTimeInstance() { CloseTimeDelay = time } };
        }
    }

    public void Clear()
    {
        MinigameTimes.Clear();
    }
}
