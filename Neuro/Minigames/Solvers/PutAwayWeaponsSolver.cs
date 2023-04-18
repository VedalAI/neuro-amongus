using Neuro.Cursor;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(StowArms))]
public sealed class PutAwayWeaponsSolver : GeneralMinigameSolver<StowArms>
{
    public override IEnumerator CompleteMinigame(StowArms minigame, NormalPlayerTask task)
    {
        if (minigame.RifleContent.active) yield return CompleteRifles(minigame);
        else yield return CompletePistols(minigame);
    }

    private IEnumerator CompleteRifles(StowArms minigame)
    {
        foreach (var rifle in minigame.RifleColliders)
        {
            yield return InGameCursor.Instance.CoMoveTo(rifle);
            InGameCursor.Instance.StartHoldingLMB(rifle);
            var slot = minigame.RifleSlots.Where(slot => slot.Occupant == null).FirstOrDefault();
            yield return InGameCursor.Instance.CoMoveTo(slot);
            InGameCursor.Instance.StopHoldingLMB();
        }
    }

    private IEnumerator CompletePistols(StowArms minigame)
    {
        foreach (var pistol in minigame.GunColliders)
        {
            yield return InGameCursor.Instance.CoMoveTo(pistol);
            InGameCursor.Instance.StartHoldingLMB(pistol);
            var slot = minigame.GunsSlots.Where(slot => slot.Occupant == null).FirstOrDefault();
            yield return InGameCursor.Instance.CoMoveTo(slot);
            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}
