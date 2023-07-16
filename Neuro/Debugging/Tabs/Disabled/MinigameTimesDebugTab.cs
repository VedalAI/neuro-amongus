using System.Collections.Generic;
using System.Linq;
using Neuro.Minigames;
using UnityEngine;

namespace Neuro.Debugging.Tabs.Disabled;

// [DebugTab]
public sealed class MinigameTimesDebugTab : DebugTab
{
    public override string Name => "Minigames";

    public override bool IsEnabled => MinigameTimeHandler.Instance;

    public override void BuildUI()
    {
        if (GUILayout.Button("Clear")) MinigameTimeHandler.Instance.Clear();
        if (GUILayout.Button("Log"))
        {
            foreach ((MinigameTimeHandler.MinigameTimeKey minigameTime, List<float> times) in MinigameTimeHandler.Instance.MinigameTimes)
            {
                float minTime = times.MinBy(t => t);
                float maxTime = times.MaxBy(t => t);
                float avgTime = times.Average(t => t);
                System.Console.Write($"{minigameTime.Type}(Step {minigameTime.Step}) {minigameTime.TimerState}");
                System.Console.Write("     ");
                System.Console.WriteLine($"range: {minTime:0.00}s - {maxTime:0.00}s (avg: {avgTime:0.00}s)");
            }
        }

        foreach ((MinigameTimeHandler.MinigameTimeKey minigameTime, List<float> times) in MinigameTimeHandler.Instance.MinigameTimes)
        {
            float minTime = times.MinBy(t => t);
            float maxTime = times.MaxBy(t => t);
            float avgTime = times.Average(t => t);
            using (new HorizontalScope())
            {
                GUILayout.Label($"{minigameTime.Type}(Step {minigameTime.Step}) {minigameTime.TimerState}");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"range: {minTime:0.00}s - {maxTime:0.00}s (avg: {avgTime:0.00}s)");
            }
        }
    }
}
