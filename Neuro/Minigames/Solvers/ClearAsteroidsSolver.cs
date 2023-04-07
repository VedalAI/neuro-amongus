using System.Collections;
using System.Linq;
using Neuro.Cursor;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WeaponsMinigame))]
public sealed class ClearAsteroidsSolver : MinigameSolver<WeaponsMinigame>
{
    protected override IEnumerator CompleteMinigame(WeaponsMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapToCenter();

        while (!task.IsComplete)
        {
            Asteroid closest = minigame.asteroidPool.activeChildren.ToArray().OfIl2CppType<Asteroid>()
                .Where(a => a && a.enabled && a.gameObject.active && a.transform.localPosition.x < 2)
                .MinBy(a => ((Vector2) InGameCursor.Instance.transform.position - (Vector2) a.transform.position).sqrMagnitude);

            if (!closest)
            {
                yield return null;
                continue;
            }

            InGameCursor.Instance.StartFollowing(closest, () => closest.gameObject.active, 0.6f);
            while (InGameCursor.Instance.IsDoingContinuousMovement && InGameCursor.Instance.DistanceToTarget is > 0.01f or -1) yield return null;

            InGameCursor.Instance.StopMovement();
            if (closest.gameObject.active)
            {
                yield return InGameCursor.Instance.CoPressLMB();
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
