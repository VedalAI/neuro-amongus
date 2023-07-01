using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WaterPlantsGame))]
public sealed class WaterPlantsSolver : GeneralMinigameSolver<WaterPlantsGame>
{
    public override float CloseTimout => 6;

    public override IEnumerator CompleteMinigame(WaterPlantsGame minigame, NormalPlayerTask task)
    {
        if (task.taskStep == 0) yield return CompleteStage1(minigame);
        else if (task.taskStep == 1) yield return CompleteStage2(minigame);
    }

    private IEnumerator CompleteStage1(WaterPlantsGame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.WaterCan);
        minigame.PickWaterCan();
    }

    private IEnumerator CompleteStage2(WaterPlantsGame minigame)
    {
        for (int index = 0; index < minigame.Plants.Count; index++)
        {
            SpriteRenderer plant = minigame.Plants[index];
            yield return InGameCursor.Instance.CoMoveTo(plant);
            minigame.WaterPlant(index);
        }
    }
}
