using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(SwitchMinigame))]
public sealed class FixLightsSolver : MinigameSolver<SwitchMinigame>
{
    public override IEnumerator CompleteMinigame(SwitchMinigame minigame, NormalPlayerTask task)
    {
        InGameCursor.Instance.SnapToCenter();
        SwitchSystem switchSystem = minigame.ship.Systems[SystemTypes.Electrical].TryCast<SwitchSystem>();
        while (switchSystem.IsActive)
        {
            var firstOff = minigame.lights
                .Select((light, index) => new { light, index })
                .First(x => x.light.color == minigame.OffColor);

            if (!firstOff.light)
            {
                yield return new WaitForFixedUpdate();
                continue;
            }

            yield return InGameCursor.Instance.CoMoveTo(minigame.switches[firstOff.index]);
            minigame.FlipSwitch(firstOff.index);
            yield return Sleep(0.1f);
        }
    }
}