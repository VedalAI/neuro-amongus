using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ShieldMinigame))]
public sealed class PrimeShieldsSolver : GeneralMinigameSolver<ShieldMinigame>
{
    public override float CloseTimout => 6;

    public override IEnumerator CompleteMinigame(ShieldMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.Shields.Count; i++)
        {
            byte b = (byte) (1 << i);
            if ((minigame.shields & b) == 0)
            {
                yield return InGameCursor.Instance.CoMoveTo(minigame.Shields[i].transform.position);
                minigame.ToggleShield(i);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
