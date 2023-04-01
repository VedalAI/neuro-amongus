using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(UploadDataGame))]
public sealed class UploadDataSolver : MinigameSolver<UploadDataGame>
{
    public override IEnumerator CompleteMinigame(UploadDataGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Button);
        minigame.Click();
    }
}
