using System.Collections;
using System.Numerics;
using HarmonyLib;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(TuneRadioMinigame))]
public class TuneRadioSabotageSolver : MinigameSolver<TuneRadioMinigame>
{
    protected override IEnumerator CompleteMinigame(TuneRadioMinigame minigame, NormalPlayerTask task)
    {
        //TODO: Make this with InGameCursor and EulerAngles?
        minigame.dial.SetValue(minigame.targetAngle);

        yield return task;
    }
}
