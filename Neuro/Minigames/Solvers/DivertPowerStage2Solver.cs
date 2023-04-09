using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AcceptDivertPowerGame))]
public sealed class DivertPowerStage2MinigameSolver : TaskMinigameSolver<AcceptDivertPowerGame>
{
    protected override IEnumerator CompleteMinigame(AcceptDivertPowerGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Switch);
        minigame.DoSwitch();
    }
}
