using Neuro.Cursor;
using Neuro.Utilities;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(RecordsMinigame))]
public sealed class SortRecordsSolver : GeneralMinigameSolver<RecordsMinigame>
{
    public override IEnumerator CompleteMinigame(RecordsMinigame minigame, NormalPlayerTask task)
    {
        if (minigame.ShelfContent.active) yield return PlaceBook(minigame);
        else if (minigame.DrawerContent.active) yield return PlaceFolder(minigame);
        else yield return GrabFolder(minigame);
    }

    private IEnumerator GrabFolder(RecordsMinigame minigame)
    {
        var target = minigame.Folders.Where(folder => folder.isVisible).Take(1).Single();
        yield return InGameCursor.Instance.CoMoveTo(target);
        minigame.GrabFolder(target);
    }

    private IEnumerator PlaceBook(RecordsMinigame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.targetBook);
        minigame.PlaceBook();
    }

    private IEnumerator PlaceFolder(RecordsMinigame minigame)
    {
        yield return InGameCursor.Instance.CoMoveTo(minigame.DrawerFolder);
        minigame.FileDocument();
    }
}
