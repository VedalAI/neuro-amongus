using Il2CppSystem.Collections.Generic;
using Neuro.Cursor;
using System.Collections;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(StartFansMinigame))]
public sealed class StartFansSolver : GeneralMinigameSolver<StartFansMinigame>
{
    private static string[] codeSprites = new string[4];

    public override IEnumerator CompleteMinigame(StartFansMinigame minigame, NormalPlayerTask task)
    {
        if (task.TaskStep == 0) yield return CompleteStage1(minigame);
        else yield return CompleteStage2(minigame);
    }

    private IEnumerator CompleteStage1(StartFansMinigame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.mainCodeButton);
        minigame.RevealCode();

        for (int index = 0; index < minigame.CodeIcons.Count; index++)
        {
            codeSprites[index] = minigame.CodeIcons[index].sprite.name;
        }

        // fake committing code to memory
        yield return new WaitForSeconds(3f);
        minigame.Close();
    }

    private IEnumerator CompleteStage2(StartFansMinigame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.mainCodeButton);
        minigame.RevealCode();
        for (int index = 0; index < minigame.CodeIcons.Count; index++)
        {
            // check if this button is already set
            if (minigame.CodeIcons[index].sprite.name == codeSprites[index]) continue;

            yield return InGameCursor.Instance.CoMoveTo(minigame.CodeIcons[index]);
            while (minigame.CodeIcons[index].sprite.name != codeSprites[index])
            {
                minigame.RotateImage(minigame.CodeIcons[index]);
                yield return new WaitForSeconds(0.25f);
            }
            
        }
    }
}
