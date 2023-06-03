using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(UploadDataGame))]
public sealed class UploadDataSolver : GeneralMinigameSolver<UploadDataGame>
{
    public override IEnumerator CompleteMinigame(UploadDataGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Button);
        minigame.Click();
    }
}
