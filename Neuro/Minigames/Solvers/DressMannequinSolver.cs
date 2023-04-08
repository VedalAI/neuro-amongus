using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DressUpMinigame))]
public class DressMannequinSolver : MinigameSolver<DressUpMinigame>
{
    protected override IEnumerator CompleteMinigame(DressUpMinigame minigame, NormalPlayerTask task)
    {
        // the hitbox placement in this minigame is terrible
        // innersloth pls fix

        DressUpCosmetic hat = minigame.buttons.First(b => b.Rend.sprite == minigame.DummyHat.sprite);
        yield return InGameCursor.Instance.CoMoveTo(hat.transform.position + new Vector3(0f, 0.5f) , 0.5f);
        InGameCursor.Instance.StartHoldingLMB(hat);
        yield return InGameCursor.Instance.CoMoveTo(minigame.hatHitbox.bounds.center + new Vector3(0f, 0.3f), 0.5f);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();

        DressUpCosmetic accessory = minigame.buttons.First(b => b.Rend.sprite == minigame.DummyAccessory.sprite);
        yield return InGameCursor.Instance.CoMoveTo(accessory, 0.5f);
        InGameCursor.Instance.StartHoldingLMB(accessory);
        yield return InGameCursor.Instance.CoMoveTo(minigame.faceHitbox, 0.5f);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();

        DressUpCosmetic clothes = minigame.buttons.First(b => b.Rend.sprite == minigame.DummyClothes.sprite);
        yield return InGameCursor.Instance.CoMoveTo(clothes.transform.position - new Vector3(0f, 0.5f), 0.5f);
        InGameCursor.Instance.StartHoldingLMB(clothes);
        yield return InGameCursor.Instance.CoMoveTo(minigame.bodyHitbox.bounds.center, 0.5f);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}
