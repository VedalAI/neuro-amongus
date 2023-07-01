using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(DressUpMinigame))]
public sealed class DressMannequinSolver : GeneralMinigameSolver<DressUpMinigame>
{
    public override float CloseTimout => 8;

    public override IEnumerator CompleteMinigame(DressUpMinigame minigame, NormalPlayerTask task)
    {
        // the hitbox placement in this minigame is terrible
        // innersloth pls fix

        const float speed = 0.8f;

        DressUpCosmetic hat = minigame.buttons.First(b => b.Rend.sprite == minigame.DummyHat.sprite);
        yield return InGameCursor.Instance.CoMoveTo(hat.transform.position + new Vector3(0f, 0.5f), speed);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.hatHitbox.bounds.center + new Vector3(0f, 0.3f), speed);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();

        DressUpCosmetic accessory = minigame.buttons.First(b => b.Rend.sprite == minigame.DummyAccessory.sprite);
        yield return InGameCursor.Instance.CoMoveTo(accessory, speed);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.faceHitbox, speed);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();

        DressUpCosmetic clothes = minigame.buttons.First(b => b.Rend.sprite == minigame.DummyClothes.sprite);
        yield return InGameCursor.Instance.CoMoveTo(clothes.transform.position - new Vector3(0f, 0.5f), speed);
        InGameCursor.Instance.StartHoldingLMB(minigame);
        yield return InGameCursor.Instance.CoMoveTo(minigame.bodyHitbox.bounds.center, speed);
        yield return new WaitForSeconds(0.1f);
        InGameCursor.Instance.StopHoldingLMB();
    }
}
