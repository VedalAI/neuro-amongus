using Il2CppInterop.Runtime.Attributes;
using System.Collections.Generic;

namespace Neuro.Minigames;

public static class MinigameTimeCollection
{
    public class MinigameTimeInstance
    {
        public float Time;
        public float CloseTimeDelay;

        public MinigameTimeInstance()
        {
            Time = 0.0f;
            CloseTimeDelay = 0.0f;
        }
    }

    [HideFromIl2Cpp]
    public static Dictionary<string, MinigameTimeInstance> MinigameTimes { get; } = new Dictionary<string, MinigameTimeInstance>();

    public static void AddMinigameTime(Minigame minigame, float time)
    {
        string name = minigame.GetIl2CppType().Name;
        if (MinigameTimes.TryGetValue(name, out MinigameTimeInstance instance))
        {
            if (instance.Time > 0.0f) return;
            instance.Time = time;
        }
        else
        {
            MinigameTimes[name] = new MinigameTimeInstance() { Time = time };
        }
    }

    public static void AddMinigameCloseTimeDelay(Minigame minigame, float time)
    {
        string name = minigame.GetIl2CppType().Name;
        if (MinigameTimes.TryGetValue(name, out MinigameTimeInstance instance))
        {
            if (instance.CloseTimeDelay > 0.0f) return;
            instance.CloseTimeDelay = time;
        }
        else
        {
            MinigameTimes[name] = new MinigameTimeInstance() { CloseTimeDelay = time };
        }
    }
}
