using System.Collections;
using System.Linq;
using Neuro.Cursor;

namespace Neuro.Minigames.Completion.Solvers;

[MinigameSolver(typeof(VendingMinigame))]
public sealed class BuyBeverageSolver : MinigameSolver<VendingMinigame>
{
    public override IEnumerator CompleteMinigame(VendingMinigame minigame, NormalPlayerTask task)
    {
        UiElement[] uiElements = minigame.ControllerSelectable.ToArray();

        yield return InGameCursor.Instance.CoMoveTo(uiElements.First(e => e.name.ToLower() == "vending_button" + minigame.targetCode[0]));
        minigame.EnterDigit(minigame.targetCode[0].ToString());
        yield return Sleep(0.1f);

        yield return InGameCursor.Instance.CoMoveTo(uiElements.First(e => e.name.ToLower() == "vending_button" + minigame.targetCode[1]));
        minigame.EnterDigit(minigame.targetCode[1].ToString());
        yield return Sleep(0.1f);

        yield return InGameCursor.Instance.CoMoveTo(uiElements.First(e => e.name.ToLower() == "admin_keypad_check"));
        minigame.AcceptDigits();
    }
}
