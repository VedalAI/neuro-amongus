using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(RewindTapeMinigame))]
public class RewindTapesSolver : GeneralMinigameSolver<RewindTapeMinigame>
{
    public override IEnumerator CompleteMinigame(RewindTapeMinigame minigame, NormalPlayerTask task)
    {
        if (minigame.currentTime > minigame.targetTime)
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.RewindButton);
            minigame.Rewind();
            yield return InGameCursor.Instance.CoMoveTo(minigame.PlayButton);

            // overshoot a little when rewinding to make it a bit more realistic
            while (minigame.currentTime - minigame.targetTime > -5f)
                yield return null;
        }
        else
        {
            yield return InGameCursor.Instance.CoMoveTo(minigame.FastFwdButton);
            minigame.FastForward();
            yield return InGameCursor.Instance.CoMoveTo(minigame.PlayButton);

            // stop a bit before the target time to make it a bit more realistic
            while (Mathf.Abs(minigame.targetTime - minigame.currentTime) > 5f)
                yield return null;
        }

        minigame.Play();
        yield return InGameCursor.Instance.CoMoveTo(minigame.PauseButton);

        // stops a second before the actual time, but this is how the game checks for it so /shrug?
        while (Mathf.Abs(minigame.targetTime - minigame.currentTime) > 1f)
            yield return null;

        minigame.Pause();
    }
}
