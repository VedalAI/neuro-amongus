using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DivertPowerMinigame))]
public sealed class DivertPowerStage1MinigameSolver : TaskMinigameSolver<DivertPowerMinigame>
{
    protected override IEnumerator CompleteMinigame(DivertPowerMinigame minigame, NormalPlayerTask task)
    {
        Collider2D slider = minigame.Sliders[minigame.sliderId];
        Vector2 position;

        yield return InGameCursor.Instance.CoMoveTo(slider);

        do
        {
            InGameCursor.Instance.SnapTo(slider);

            position = slider.transform.localPosition;
            position.y = minigame.SliderY.Clamp(position.y + Time.deltaTime * 5f);
            slider.transform.localPosition = position;

            yield return null;
        }
        while (minigame.SliderY.max - position.y >= 0.01f);

        minigame.prevHadInput = true;
    }
}
