using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AirshipUploadGame))]
public sealed class AirshipUploadSolver : MinigameSolver<AirshipUploadGame>
{
    protected override IEnumerator CompleteMinigame(AirshipUploadGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Phone);
        minigame.phoneGrabbed = true;

        yield return InGameCursor.Instance.CoMoveTo(minigame.Hotspot);
        minigame.phoneGrabbed = false;
    }
}
