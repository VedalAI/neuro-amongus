using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Neuro.Debugging;

public sealed class MainDebugWindow : IDebugWindow
{
    public void RegisterTabs(DebugWindowBehaviour behaviour)
    {
        behaviour.RegisterTab("Tasks", BuildTasksWindow, ShouldShowTasksWindow);
    }

    private static bool ShouldShowTasksWindow()
    {
        return PlayerControl.LocalPlayer && TutorialManager.InstanceExists;
    }

    private static void BuildTasksWindow()
    {
        if (GUILayout.Button("Open Task Picker"))
        {
            Minigame minigamePrefab = ShipStatus.Instance.GetComponentsInChildren<SystemConsole>().First(c => c.FreeplayOnly).MinigamePrefab;
            PlayerControl.LocalPlayer.NetTransform.Halt();
            Minigame minigame = Object.Instantiate(minigamePrefab, Camera.main!.transform, false);
            minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
            minigame.Begin(null);
        }
    }
}
