using Neuro.Cursor;
using System.Collections;
using System.Linq;
using Neuro.Extensions;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(RecordsMinigame))]
public sealed class SortRecordsSolver : GeneralMinigameSolver<RecordsMinigame>
{
    public override float CloseTimout => 5;

    public override IEnumerator CompleteMinigame(RecordsMinigame minigame, NormalPlayerTask task)
    {
        if (minigame.ShelfContent.active) yield return PlaceBook(minigame);
        else if (minigame.DrawerContent.active) yield return PlaceFolder(minigame);
        else yield return GrabFolder(minigame);
    }

    private IEnumerator GrabFolder(RecordsMinigame minigame)
    {
        SpriteRenderer target = minigame.Folders.ReverseSection(1..3).First(folder => folder.gameObject.active);
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