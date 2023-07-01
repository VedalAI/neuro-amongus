using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(PolishRubyGame))]
public sealed class PolishRubySolver : GeneralMinigameSolver<PolishRubyGame>
{
    public override float CloseTimout => 11;

    public override IEnumerator CompleteMinigame(PolishRubyGame minigame, NormalPlayerTask task)
    {
        Vector3 rubOffset = new(0.1f, 0.1f);

        foreach (PassiveButton button in minigame.Buttons)
        {
            if (!button.isActiveAndEnabled) continue;

            yield return InGameCursor.Instance.CoMoveTo(button);

            InGameCursor.Instance.StartHoldingLMB(minigame);
            SpriteRenderer renderer = button.GetComponent<SpriteRenderer>();
            while (renderer.color != Palette.ClearWhite)
            {
                yield return InGameCursor.Instance.CoMoveTo(button.transform.position + rubOffset);
                yield return InGameCursor.Instance.CoMoveTo(button.transform.position - rubOffset);
            }

            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}
