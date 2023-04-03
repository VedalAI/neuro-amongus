using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(SwitchMinigame))]
public sealed class FixLightsSolver : MinigameSolver<SwitchMinigame>
{
    public override IEnumerator CompleteMinigame(SwitchMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.switches.Count; i++)
        {
            if (minigame.lights[i].color == minigame.OffColor)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.switches[i]);
                minigame.FlipSwitch(i);
                yield return Sleep(0.1f);
            }
        }
    }
}
