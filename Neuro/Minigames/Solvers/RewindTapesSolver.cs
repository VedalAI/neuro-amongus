using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(RewindTapeMinigame))]
public sealed class RewindTapesSolver : GeneralMinigameSolver<RewindTapeMinigame>
{
    public override float CloseTimout => 25;

    public override IEnumerator CompleteMinigame(RewindTapeMinigame minigame, NormalPlayerTask task)
    {
        if (minigame.currentTime > minigame.targetTime)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.RewindButton);
            minigame.Rewind();
            yield return new WaitForSeconds(0.5f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.PlayButton, 0.5f);

            while (minigame.currentTime - minigame.targetTime > -2.5f) yield return null;
        }
        else
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.FastFwdButton);
            minigame.FastForward();
            yield return new WaitForSeconds(0.5f);
            yield return InGameCursor.Instance.CoMoveTo(minigame.PlayButton, 0.5f);

            while (minigame.targetTime - minigame.currentTime > 2.5f) yield return null;
        }

        minigame.Play();
        yield return new WaitForSeconds(0.5f);
        yield return InGameCursor.Instance.CoMoveTo(minigame.PauseButton, 0.5f);

        while (minigame.targetTime - minigame.currentTime > -0.25f) yield return null;

        minigame.Pause();
    }
}
