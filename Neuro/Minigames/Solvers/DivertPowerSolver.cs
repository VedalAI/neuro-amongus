using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(AcceptDivertPowerGame), false)]
[MinigameSolver(typeof(DivertPowerMinigame), false)]
[MinigameOpener(typeof(DivertPowerMetagame))]
public sealed class DivertPowerSolver : IMinigameSolver<Minigame>, IMinigameOpener
{
    public float CloseTimout => 4;

    public bool ShouldOpenConsole(Console console, PlayerTask task) => true;

    public IEnumerator CompleteMinigame(Minigame minigame)
    {
        if (minigame.TryCast<DivertPowerMinigame>() is { } part1)
            yield return CompleteStage1(part1);
        else
            yield return CompleteStage2(minigame.Cast<AcceptDivertPowerGame>());
    }

    private IEnumerator CompleteStage1(DivertPowerMinigame minigame) // TODO: fIX THIS IS BROKEN SOMETINMES
    {
        Collider2D slider = minigame.Sliders[minigame.sliderId];

        yield return InGameCursor.Instance.CoMoveTo(slider);
        InGameCursor.Instance.StartHoldingLMB(minigame);

        Vector3 destination = slider.transform.parent.TransformPoint(new Vector3(slider.transform.localPosition.x, minigame.SliderY.max + 0.2f));
        yield return InGameCursor.Instance.CoMoveTo(destination, 0.5f);
        InGameCursor.Instance.StopHoldingLMB();
    }

    private IEnumerator CompleteStage2(AcceptDivertPowerGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.Switch);
        minigame.DoSwitch();
    }
}
