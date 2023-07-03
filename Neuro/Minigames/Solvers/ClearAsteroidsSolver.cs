using System.Collections;
using System.Linq;
using Neuro.Cursor;
using Neuro.Extensions;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(WeaponsMinigame))]
public sealed class ClearAsteroidsMinigameSolver : GeneralMinigameSolver<WeaponsMinigame>
{
    public override float CloseTimout => 25f;

    public override IEnumerator CompleteMinigame(WeaponsMinigame minigame, NormalPlayerTask task)
    {
        while (!task.IsComplete)
        {
            Asteroid closest = minigame.asteroidPool.activeChildren.ToArray().OfIl2CppType<Asteroid>()
                .Where(a => a && a.enabled && a.gameObject.active)
                .Where(a => a.transform.localPosition is {x: < 1.9f, y: > -1.9f and < 1.9f})
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