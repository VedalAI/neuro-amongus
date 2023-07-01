using System.Collections;
using System.Linq;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WireMinigame))]
public sealed class FixWiringSolver : GeneralMinigameSolver<WireMinigame>
{
    public override float CloseTimout => 10;

    public override IEnumerator CompleteMinigame(WireMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.LeftNodes.Count; i++)
        {
            Wire left = minigame.LeftNodes[i];
            yield return InGameCursor.Instance.CoMoveTo(left);
            InGameCursor.Instance.StartHoldingLMB(minigame);

            WireNode right = minigame.RightNodes.First(x => x.WireId == minigame.ExpectedWires[i]);
            yield return InGameCursor.Instance.CoMoveTo(right);
            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}
