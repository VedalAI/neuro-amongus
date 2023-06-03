using Neuro.Debugging;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames;

[DebugTab]
public sealed class MinigameTimesDebugTab : DebugTab
{
    public override string Name => "Minigame Times";

    public override bool IsEnabled => true; // MinigameTimeCollection.MinigameTimes != null;

    public override void BuildUI()
    {
        foreach (var minigameTime in MinigameTimeCollection.MinigameTimes)
        {
            using (new HorizontalScope())
            {
                GUILayout.Label($"{minigameTime.Key}");
                GUILayout.FlexibleSpace();
                GUILayout.Label($"time: {minigameTime.Value.Time:0.00} close time: {minigameTime.Value.CloseTimeDelay:0.00} total: {(minigameTime.Value.Time + minigameTime.Value.CloseTimeDelay):0.00}");
            }
        }
    }
}
