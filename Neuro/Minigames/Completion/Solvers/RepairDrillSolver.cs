using System.Collections;
using Neuro.Cursor;
using PowerTools;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(DrillMinigame))]
public sealed class RepairDrillSolver : MinigameSolver<DrillMinigame>
{
    public override IEnumerator CompleteMinigame(DrillMinigame minigame, NormalPlayerTask task)
    {
        int buttonIndex = 0;
        while (buttonIndex < minigame.Buttons.Length)
        {
            SpriteAnim button = minigame.Buttons[buttonIndex];

            InGameCursor.Instance.MoveTo(button);
            minigame.FixButton(button);
            yield return new WaitForSeconds(0.1f * DELAY_MULTIPLIER);

            if (minigame.states[buttonIndex] == minigame.MaxState) buttonIndex++;
        }
    }
}
