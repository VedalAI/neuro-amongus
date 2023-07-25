using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Extensions;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.Tasks;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class TasksRecorder : MonoBehaviour
{
    public static TasksRecorder Instance { get; private set; }

    public TasksRecorder(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp] public TasksFrame Frame { get; } = new();

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;

        /*Console closestConsole = null;
        float closestDistance = 999f;*/

        Frame.Tasks.Clear();
        Frame.Sabotage = null;

        if (MovementSuggestion.Instance && MovementSuggestion.Instance.CurrentTarget)
        {
            TaskData fakeData = TaskData.CreateFake(MovementSuggestion.Instance.CurrentTarget);
            Frame.Tasks.AddRange(Enumerable.Repeat(fakeData, 10));
            Frame.Sabotage = fakeData;

            return;
        }

        foreach (NormalPlayerTask task in PlayerControl.LocalPlayer.myTasks.ToArray().OfIl2CppType<NormalPlayerTask>().Where(t => !t.IsComplete))
        {
            TaskData data = TaskData.Create(task);
            Frame.Tasks.Add(data);

            /*foreach (Console console in task.FindConsoles())
            {
                if (!console) continue;
                var distance = PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, console, console);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestConsole = console;
                }
            }*/
        }

        /*if(closestConsole != null)
        {
            var path = PathfindingHandler.Instance.GetPath(PlayerControl.LocalPlayer, closestConsole, closestConsole);

            TaskData.DrawPath(path, closestConsole);
        }*/

        PlayerTask sabotage = PlayerControl.LocalPlayer.myTasks._items.FirstOrDefault(s => PlayerTask.TaskIsEmergency(s) && !s.IsComplete);
        if (sabotage) Frame.Sabotage = TaskData.Create(sabotage);
    }
}
