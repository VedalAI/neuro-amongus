using Neuro.Cursor;
using System.Collections;
using System.Linq;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(WireMinigame))]
public sealed class FixWiringSolver : MinigameSolver<WireMinigame>
{
    public override IEnumerator CompleteMinigame(WireMinigame minigame, NormalPlayerTask task)
    {
        for (int i = 0; i < minigame.LeftNodes.Count; i++)
        {
            Wire left = minigame.LeftNodes[i];
            yield return InGameCursor.Instance.CoMoveTo(left.transform.position);
            minigame.prevSelectedWireIndex = i;
            WireNode right = minigame.RightNodes.First(x => x.WireId == minigame.ExpectedWires[i]);
            yield return InGameCursor.Instance.CoMoveTo(right.transform.position);
            left.ConnectRight(right);
            // TODO: Make the wire actually follow the cursor. Should eliminate the need to do this
            minigame.ActualWires[i] = minigame.ExpectedWires[i];
            minigame.UpdateLights();
        }
        minigame.CheckTask();
    }
}
