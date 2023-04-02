using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(ShieldMinigame))]
public sealed class PrimeShieldsSolver : MinigameSolver<ShieldMinigame>
{
    public override IEnumerator CompleteMinigame(ShieldMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.Shields.Count; i++)
        {
            if (minigame.Shields[i].color == minigame.OffColor)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.Shields[i].transform.position);
                minigame.ToggleShield(i);
                yield return Sleep(0.1f);
            }
        }
    }
}

