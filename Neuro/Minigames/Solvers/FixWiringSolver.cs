using Neuro.Cursor;
using System.Collections;
using System.Linq;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(WireMinigame))]
public sealed class FixWiringSolver : MinigameSolver<WireMinigame>
{
    protected override IEnumerator CompleteMinigame(WireMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.LeftNodes.Count; i++)
        {
            Wire left = minigame.LeftNodes[i];
            yield return InGameCursor.Instance.CoMoveTo(left);
            InGameCursor.Instance.IsMouseDown = true;
            WireNode right = minigame.RightNodes.First(x => x.WireId == minigame.ExpectedWires[i]);
            yield return InGameCursor.Instance.CoMoveTo(right);
            InGameCursor.Instance.IsMouseDown = false;
        }
    }
}
