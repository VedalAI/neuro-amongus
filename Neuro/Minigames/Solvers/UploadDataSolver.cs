using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(UploadDataGame))]
public sealed class UploadDataSolver : GeneralMinigameSolver<UploadDataGame>
{
    public override float CloseTimout => 16;

    public override IEnumerator CompleteMinigame(UploadDataGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Button);
        minigame.Click();
    }
}