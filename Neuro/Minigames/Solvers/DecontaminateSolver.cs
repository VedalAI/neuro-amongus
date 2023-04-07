using System.Collections;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(ShowerMinigame))]
public class DecontaminateSolver : MinigameSolver<ShowerMinigame>
{
    protected override IEnumerator CompleteMinigame(ShowerMinigame minigame, NormalPlayerTask task)
    {
        //       a programmer's worst nightmare       ^^^^^^^^^

        // basically we just sit and wait for it to finish because we don't have to do anything
        while (!task.IsComplete)
            yield return null;
    }
}
