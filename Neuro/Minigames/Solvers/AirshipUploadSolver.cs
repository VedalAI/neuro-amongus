using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipUploadGame))]
public sealed class AirshipUploadSolver : MinigameSolver<AirshipUploadGame>
{
    protected override IEnumerator CompleteMinigame(AirshipUploadGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Phone);
        InGameCursor.Instance.StartHoldingLMB(task);

        yield return InGameCursor.Instance.CoMoveTo(minigame.Hotspot);
        InGameCursor.Instance.StopHolding();
    }
}
