using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(BoardPassGame))]
public sealed class ScanBoardingPassSolver : MinigameSolver<BoardPassGame>
{
    public override IEnumerator CompleteMinigame(BoardPassGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.pullButton);
        yield return minigame.CoPullPass();
        yield return InGameCursor.Instance.CoMoveTo(minigame.flipButton);
        yield return minigame.CoFlipPass();
        yield return InGameCursor.Instance.CoMoveTo(minigame.pass);
        InGameCursor.Instance.StartFollowing(minigame.pass);

        yield return Sleep(0.1f);

        Vector3 originalPosition = minigame.pass.transform.localPosition;
        for (float t = 0; t < 0.2f; t += Time.deltaTime)
        {
            minigame.pass.transform.localPosition = (Vector3) Vector2.Lerp(originalPosition, minigame.Sensor.transform.localPosition, t / 0.2f) with {z = originalPosition.z};
            yield return null;
        }
        minigame.pass.transform.localPosition = minigame.Sensor.transform.localPosition with {z = originalPosition.z};
    }
}
