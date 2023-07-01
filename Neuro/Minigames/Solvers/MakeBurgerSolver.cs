using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(BurgerMinigame))]
public sealed class MakeBurgerSolver : GeneralMinigameSolver<BurgerMinigame>
{
    public override float CloseTimout => 12;

    public override IEnumerator CompleteMinigame(BurgerMinigame minigame, NormalPlayerTask task)
    {
        yield return new WaitForSeconds(0.5f);
        minigame.TogglePaper();

        foreach (BurgerToppingTypes topping in minigame.ExpectedToppings)
        {
            yield return new WaitForSeconds(0.2f);

            // apparently the plate is a topping
            if (topping == BurgerToppingTypes.Plate)
                continue;

            BurgerTopping target = minigame.Toppings
                .Where(t => !minigame.burger.Contains(t))
                .First(t => t.ToppingType == topping);

            yield return InGameCursor.Instance.CoMoveTo(target, 0.75f);
            InGameCursor.Instance.StartHoldingLMB(minigame);
            yield return InGameCursor.Instance.CoMoveTo(minigame.burger.Peek(), 0.75f);
            InGameCursor.Instance.StopHoldingLMB();
        }
    }
}
