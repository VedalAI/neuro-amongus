using System.Collections;
using System.Linq;
using Neuro.Cursor;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WeaponsMinigame))]
public sealed class ClearAsteroidsSolver : MinigameSolver<WeaponsMinigame>
{
    public override IEnumerator CompleteMinigame(WeaponsMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapToCenter();
        minigame.BackgroundCol.bounds.Expand(-50);

        while (!task.IsComplete)
        {
            Asteroid closest = minigame.asteroidPool.activeChildren.ToArray().OfIl2CppType<Asteroid>()
                .Where(a => a && a.enabled && a.gameObject.active && a.transform.position.x < 10)
                .MinBy(a => ((Vector2) InGameCursor.Instance.transform.position - (Vector2) a.transform.position).magnitude);

            if (!closest)
            {
                yield return null;
                continue;
            }

            InGameCursor.Instance.StartFollowing(closest, () => closest.gameObject.active, 0.6f);
            while (InGameCursor.Instance.IsDoingContinuousMovement && InGameCursor.Instance.DistanceToTarget is > 0.5f or -1) yield return null;
            yield return null;

            if (closest.gameObject.active)
            {
                minigame.BreakApart(closest);
                DoMinigameAnimation(minigame);
                yield return Sleep(0.25f);
            }

            // TODO: Figure out why DoMinigameAnimation doesnt place the visor at the right spot and places it right behind the actual cursor position
        }
    }

    // Taken straight from WeaponsMinigame.FixedUpdate
    private void DoMinigameAnimation(WeaponsMinigame minigame)
    {
        if (Constants.ShouldPlaySfx())
        {
            SoundManager.Instance.PlaySound(minigame.ShootSound, false);
        }
        Vector3 vector3 = (Vector3) InGameCursor.Instance.Position - minigame.transform.position;
        vector3.z = -2f;
        minigame.TargetReticle.transform.localPosition = vector3;
        vector3.z = 0f;
        minigame.TargetLines.SetPosition(1, vector3);
        if (ShipStatus.Instance.WeaponsImage && !ShipStatus.Instance.WeaponsImage.IsPlaying())
        {
            PlayerControl.LocalPlayer.RpcPlayAnimation(6);
        }
    }
}
