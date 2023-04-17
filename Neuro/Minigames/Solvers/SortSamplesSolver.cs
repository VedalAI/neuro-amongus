using Neuro.Cursor;

using System.Collections;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SortMinigame))]
public sealed class SortSamplesSolver : GeneralMinigameSolver<SortMinigame>
{
    public override IEnumerator CompleteMinigame(SortMinigame minigame, NormalPlayerTask task)
    {
        foreach (var obj in minigame.Objects)
        {
            yield return InGameCursor.Instance.CoMoveTo(obj);
            InGameCursor.Instance.StartHoldingLMB(minigame);

            switch (obj.MyType)
            {
                case SortGameObject.ObjType.Plant:
                    yield return InGameCursor.Instance.CoMoveTo(minigame.PlantBox); break;
                case SortGameObject.ObjType.Animal:
                    yield return InGameCursor.Instance.CoMoveTo(minigame.AnimalBox); break;
                case SortGameObject.ObjType.Mineral:
                    yield return InGameCursor.Instance.CoMoveTo(minigame.MineralBox); break;
            }

            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}
