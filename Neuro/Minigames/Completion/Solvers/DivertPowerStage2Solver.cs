using System.Collections;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(AcceptDivertPowerGame))]
public sealed class DivertPowerStage2Solver : MinigameSolver<AcceptDivertPowerGame>
{
    public override IEnumerator CompleteMinigame(AcceptDivertPowerGame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Switch);
        minigame.DoSwitch();
    }
}
