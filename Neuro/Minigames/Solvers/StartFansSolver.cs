using Neuro.Cursor;
using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(StartFansMinigame))]
public sealed class StartFansSolver : IMinigameSolver<StartFansMinigame, NormalPlayerTask>, IMinigameOpener<NormalPlayerTask>
{
    public float CloseTimout => 8.5f;

    public bool ShouldOpenConsole(Console console, NormalPlayerTask task)
    {
        return task.TaskStep == console.ConsoleId;
    }

    public IEnumerator CompleteMinigame(StartFansMinigame minigame, NormalPlayerTask task)
    {
        if (task.TaskStep == 0) yield return CompleteStage1(minigame);
        else yield return CompleteStage2(minigame, task);
    }

    private IEnumerator CompleteStage1(StartFansMinigame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.mainCodeButton);
        minigame.RevealCode();

        // fake committing code to memory
        yield return new WaitForSeconds(1.5f);
        minigame.Close();
    }

    private IEnumerator CompleteStage2(StartFansMinigame minigame, NormalPlayerTask task)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.mainCodeButton);
        minigame.RevealCode();
        yield return new WaitForSeconds(0.25f);

        for (int index = 0; index < minigame.CodeIcons.Count; index++)
        {
            // allow skipping over already set codes
            if (minigame.CodeIcons[index].sprite == minigame.IconSprites[task.Data[index]]) continue;

            yield return InGameCursor.Instance.CoMoveTo(minigame.CodeIcons[index]);
            while (minigame.CodeIcons[index].sprite != minigame.IconSprites[task.Data[index]])
            {
                minigame.RotateImage(minigame.CodeIcons[index]);
                yield return new WaitForSeconds(0.3f);
            }
        }
    }
}