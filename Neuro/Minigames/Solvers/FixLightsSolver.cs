using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(SwitchMinigame))]
public sealed class FixLightsSolver : TasklessMinigameSolver<SwitchMinigame>
{
    protected override IEnumerator CompleteMinigame(SwitchMinigame minigame)
    {
        SwitchSystem switchSystem = minigame.ship.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
        while (switchSystem.IsActive)
        {
            (SpriteRenderer firstLight, int firstIndex) = minigame.lights
                .Select((light, index) => (light, index))
                .FirstOrDefault(t => t.light.color == minigame.OffColor);

            if (!firstLight)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            yield return InGameCursor.Instance.CoMoveTo(minigame.switches[firstIndex]);
            minigame.FlipSwitch(firstIndex);
            yield return new WaitForSeconds(0.1f);
        }
    }
}
