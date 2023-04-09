using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(PolishRubyGame))]
public class PolishRubySolver : TaskMinigameSolver<PolishRubyGame>
{
    protected override IEnumerator CompleteMinigame(PolishRubyGame minigame, NormalPlayerTask task)
    {
        var buttons = minigame.Buttons
            .Select((button, index) => (button, index))
            .Where(b => b.button.isActiveAndEnabled)
            .ToArray();

        Vector3 moveDistance = new(0f, 0.2f, 0f);
        int lastMoveDir = 1;
        foreach (var b in buttons)
        {
            PassiveButton button = b.button;
            yield return InGameCursor.Instance.CoMoveTo(button);
            SpriteRenderer sprite = button.GetComponent<SpriteRenderer>();
            for (int i = 0; i < minigame.swipesToClean; i++)
            {
                yield return InGameCursor.Instance.CoMoveTo(button.transform.position + (moveDistance * lastMoveDir));
                lastMoveDir = -lastMoveDir;
                // easier to just mimic the rubbing ourselves, minigame sometimes misses fake cursor input
                // taken from PolishRubyGame.Update
                if (Constants.ShouldPlaySfx())
                {
                    SoundManager.Instance.PlaySoundImmediate(minigame.rubSounds[Random.Range(0, minigame.rubSounds.Count)], false, 1f, 1f, null);
                }
                int num = i + 1;
                if (num <= minigame.swipesToClean)
                {
                    sprite.color = Color.Lerp(Color.white, Palette.ClearWhite, (float)num / minigame.swipesToClean);
                    if (num == minigame.swipesToClean)
                    {
                        minigame.Sparkles[b.index].enabled = true;
                    }
                }
                yield return new WaitForSeconds(0.1f);
            }
        }
        minigame.MyNormTask.NextStep();
        minigame.Close();
    }
}
