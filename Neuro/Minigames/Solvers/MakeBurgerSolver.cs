using System.Collections;
using System.Linq;
using Neuro.Cursor;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(BurgerMinigame))]
public class MakeBurgerSolver : MinigameSolver<BurgerMinigame>
{
    protected override IEnumerator CompleteMinigame(BurgerMinigame minigame, NormalPlayerTask task)
    {
        yield return Sleep(1.5f);
        minigame.TogglePaper();

        foreach (BurgerToppingTypes topping in minigame.ExpectedToppings)
        {
            // apparently the plate is a topping
            if (topping == BurgerToppingTypes.Plate)
                continue;

            BurgerTopping target = minigame.Toppings
                .Where(t => !minigame.burger.Contains(t))
                .FirstOrDefault(t => t.ToppingType == topping);

            if (!target)
            {
                Error("Failed to find any valid toppings for type " + topping);
                yield break;
            }

            yield return InGameCursor.Instance.CoMoveTo(target);
            InGameCursor.Instance.StartHoldingLMB(target);
            yield return InGameCursor.Instance.CoMoveTo(minigame.burger.Peek());
            InGameCursor.Instance.StopHolding();
            yield return Sleep(0.2f);
        }
    }
}