using System.Collections;
using System.Linq;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WireMinigame))]
public sealed class FixWiringSolver : MinigameSolver<WireMinigame>
{
    protected override IEnumerator CompleteMinigame(WireMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.LeftNodes.Count; i++)
        {
            Wire left = minigame.LeftNodes[i];
            yield return InGameCursor.Instance.CoMoveTo(left);
            minigame.prevSelectedWireIndex = i;
            WireNode right = minigame.RightNodes.First(x => x.WireId == minigame.ExpectedWires[i]);
            yield return InGameCursor.Instance.CoMoveTo(right);
            left.ConnectRight(right);
            // TODO: Make the wire actually follow the cursor. Should eliminate the need to do this
            minigame.ActualWires[i] = minigame.ExpectedWires[i];
            minigame.UpdateLights();
        }
        minigame.CheckTask();
    }
}
