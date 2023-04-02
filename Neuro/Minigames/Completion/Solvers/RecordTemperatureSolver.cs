using Neuro.Cursor;
using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(TempMinigame))]
public sealed class RecordTemperatureSolver : MinigameSolver<TempMinigame>
{
    public override IEnumerator CompleteMinigame(TempMinigame minigame, NormalPlayerTask task)
    {
        int direction = minigame.logValue < minigame.readingValue ? 1 : -1;
        Vector3 position = minigame.LogText.transform.position + new Vector3(0f, direction == 1 ? 0.7f : -0.7f, 0f);
        yield return InGameCursor.Instance.CoMoveTo(position);
        do
        {
            minigame.ChangeNumber(direction);
            yield return Sleep(0.1f);
        } while (minigame.logValue != minigame.readingValue);
    }
}

