using System.Linq;
using Neuro.Debugging;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Minigames;

[DebugTab]
public sealed class MinigameDebugTab : DebugTab
{
    public override string Name => "Tasks";

    public override void BuildUI()
    {
        if (GUILayout.Button("Open Task Picker"))
        {
            Minigame minigamePrefab = ShipStatus.Instance.GetComponentsInChildren<SystemConsole>().First(c => c.FreeplayOnly).MinigamePrefab;
            PlayerControl.LocalPlayer.NetTransform.Halt();
            Minigame minigame = Object.Instantiate(minigamePrefab, Camera.main!.transform, false);
            minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
            minigame.Begin(null);
        }

        foreach (NormalPlayerTask task in PlayerControl.LocalPlayer.myTasks.ToArray().OfIl2CppType<NormalPlayerTask>().Where(t => !t.IsComplete))
        {
            if (GUILayout.Button(TranslationController.Instance.GetString(task.TaskType)))
            {
                Console console = ShipStatus.Instance.AllConsoles.First(c => c.FindTask(PlayerControl.LocalPlayer).Id == task.Id);

                Minigame minigame = Object.Instantiate(task.GetMinigamePrefab(), Camera.main!.transform, false);
                minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                minigame.Console = console;
                minigame.Begin(task);
            }
        }
    }
}
