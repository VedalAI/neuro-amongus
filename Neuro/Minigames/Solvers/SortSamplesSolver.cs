using System.Collections;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SortMinigame))]
public class SortSamplesSolver : MinigameSolver<SortMinigame>
{
    protected override IEnumerator CompleteMinigame(SortMinigame minigame, NormalPlayerTask task)
    {
        foreach (SortGameObject obj in minigame.Objects)
        {
            yield return InGameCursor.Instance.CoMoveTo(obj);
            InGameCursor.Instance.StartHoldingLMB(obj);
            switch (obj.MyType)
            {
                case SortGameObject.ObjType.Plant:
                    yield return InGameCursor.Instance.CoMoveTo(minigame.PlantBox);
                    break;
                case SortGameObject.ObjType.Animal:
                    yield return InGameCursor.Instance.CoMoveTo(minigame.AnimalBox);
                    break;
                case SortGameObject.ObjType.Mineral:
                    yield return InGameCursor.Instance.CoMoveTo(minigame.MineralBox);
                    break;
            }
            InGameCursor.Instance.StopHoldingLMB();
            yield return new WaitForSeconds(0.1f);
        }
    }
}
