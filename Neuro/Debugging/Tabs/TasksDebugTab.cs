using System.Linq;
using System.Text.RegularExpressions;
using Il2CppSystem.Text;
using Neuro.Caching;
using Neuro.Extensions;
using UnityEngine;

namespace Neuro.Debugging.Tabs;

[DebugTab]
public sealed class TasksDebugTab : DebugTab
{
    private static readonly Regex _colorRegex = new(@"<\/?color(?:=#\w+?)?>", RegexOptions.Compiled);

    public override string Name => "Tasks";

    public override bool IsEnabled => ShipStatus.Instance && PlayerControl.LocalPlayer && PlayerControl.LocalPlayer.myTasks != null;

    public override void BuildUI()
    {
        if (GUILayout.Button("Open Task Picker"))
        {
            if (Minigame.Instance) Minigame.Instance.ForceClose();

            Minigame minigamePrefab = ShipStatus.Instance.GetComponentsInChildren<SystemConsole>().First(c => c.FreeplayOnly).MinigamePrefab;
            PlayerControl.LocalPlayer.NetTransform.Halt();
            Minigame minigame = Object.Instantiate(minigamePrefab, UnityCache.MainCamera.transform, false);
            minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
            minigame.Begin(null);
        }

        GUILayoutUtils.HorizontalDivider();

        foreach (NormalPlayerTask task in PlayerControl.LocalPlayer.myTasks.ToArray().OfIl2CppType<NormalPlayerTask>().Where(t => !t.IsComplete))
        {
            StringBuilder builder = new();
            task.AppendTaskText(builder);
            if (GUILayout.Button(_colorRegex.Replace(builder.ToString(), "").Trim()))
            {
                if (Minigame.Instance) Minigame.Instance.ForceClose();

                Console console = ShipStatus.Instance.AllConsoles.First(task.ValidConsole);

                Minigame minigame = Object.Instantiate(task.GetMinigamePrefab(), UnityCache.MainCamera.transform, false);
                minigame.transform.localPosition = new Vector3(0f, 0f, -50f);
                minigame.Console = console;
                minigame.Begin(task);
            }
        }
    }
}