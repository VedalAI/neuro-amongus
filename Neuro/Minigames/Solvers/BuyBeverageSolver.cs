using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(VendingMinigame))]
public sealed class BuyBeverageMinigameSolver : GeneralMinigameSolver<VendingMinigame>
{
    public override float CloseTimout => 10f;

    public override IEnumerator CompleteMinigame(VendingMinigame minigame, NormalPlayerTask task)
    {
        UiElement[] uiElements = minigame.ControllerSelectable.ToArray();

        yield return InGameCursor.Instance.CoMoveTo(uiElements.First(e => e.name.ToLower() == "vending_button" + minigame.targetCode[0]));
        minigame.EnterDigit(minigame.targetCode[0].ToString());
        yield return new WaitForSeconds(0.1f);

        yield return InGameCursor.Instance.CoMoveTo(uiElements.First(e => e.name.ToLower() == "vending_button" + minigame.targetCode[1]));
        minigame.EnterDigit(minigame.targetCode[1].ToString());
        yield return new WaitForSeconds(0.1f);

        yield return InGameCursor.Instance.CoMoveTo(uiElements.First(e => e.name.ToLower() == "admin_keypad_check"));
        minigame.AcceptDigits();
    }
}
