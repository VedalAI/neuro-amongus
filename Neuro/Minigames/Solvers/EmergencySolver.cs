using System.Collections;
using System.Linq;
using Neuro.Cursor;
using UnityEngine;

namespace Neuro.Minigames.Solvers;

[MinigameSolver(typeof(EmergencyMinigame))]
public sealed class EmergencySolver : IMinigameSolver<EmergencyMinigame>
{
    public float CloseTimout => 3;

    // since Emergency Button is a SystemConsole, we cannot use ShouldOpenConsole
    // we also do not have plans to allow opening other system consoles (cams, admin table, etc)
    // so just use a static function to check if we can use the button
    public static bool ShouldOpenEmergency()
    {
        return false && (!PlayerControl.LocalPlayer.myTasks._items.Any(PlayerTask.TaskIsEmergency)) &&
               PlayerControl.LocalPlayer.RemainingEmergencies > 0 &&
               // TODO: let AI govern when we want to call meeting so we dont just call a meeting every time we walk past
               // Random.RandomRange(0f, 1f) < 0.1f &&
               ShipStatus.Instance.Timer >= 30f &&
               ShipStatus.Instance.EmergencyCooldown <= 0f;
    }

    public IEnumerator CompleteMinigame(EmergencyMinigame minigame)
    {
        // wait so we don't instantly press the button upon opening
        yield return new WaitForSeconds(0.2f);
        yield return InGameCursor.Instance.CoMoveToCenter();
        minigame.CallMeeting();
    }
}
