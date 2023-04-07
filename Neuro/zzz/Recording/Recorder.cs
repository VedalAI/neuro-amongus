using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Recording.DataStructures;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

[RegisterInIl2Cpp]
public sealed class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    public Recorder(IntPtr ptr) : base(ptr) { }

    [HideFromIl2Cpp]
    public List<Frame> Frames { get; set; } = new();

    // TODO: Maybe there's a better way to store this than to keep random properties in this class
    public bool DidReport { get; set; }
    public bool DidVent { get; set; }
    public bool DidKill { get; set; }
    public bool DidSabotage { get; set; }
    public bool DidDoors { get; set; }
    public SystemTypes SabotageUsed { get; set; }
    public List<PlainDoor> DoorsUsed { get; set; } = new List<PlainDoor>();

    private int _fixedUpdateCalls = 0;

    private void Awake()
    {
        if (Instance)
        {
            LogUtils.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
        EventManager.RegisterHandler(this);
    }

    private void FixedUpdate()
    {
        // TODO: Figure out how to actually do this properly

        if (!ShipStatus.Instance) return;
        if (MeetingHud.Instance) return;
        if (Minigame.Instance) return;
        if (!PlayerControl.LocalPlayer) return;

        _fixedUpdateCalls++;
        if (_fixedUpdateCalls < 9) return;
        _fixedUpdateCalls = 0;

        // For each task, give the relative direction to nearest node plus total distance
        // TODO: Record all of the tasks
        // TODO: Record 11th task as emergency
        // TODO: Record fellow impostors
        // TODO: Record localplayer velocity
        // TODO: Raycast for obstacles

        // Record values
        Frame frame = new(
            PlayerControl.LocalPlayer.Data.Role.IsImpostor,
            PlayerControl.LocalPlayer.killTimer,
            NeuroPlugin.Instance.Movement.DirectionToNearestTask,
            PlayerControl.LocalPlayer.myTasks.ToArray().Any(PlayerTask.TaskIsEmergency),
            NeuroPlugin.Instance.Impostor.DirectionToNearestVent,
            NeuroPlugin.Instance.Impostor.NearbyVents,
            NeuroPlugin.Instance.Vision.DirectionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.isActiveAndEnabled,
            NeuroPlugin.Instance.Vision.PlayerRecords.ToDictionary(kv => kv.Key.PlayerId, kv => kv.Value),
            NeuroPlugin.Instance.Movement.LastMoveDirection,
            DidReport,
            DidVent,
            PlayerControl.LocalPlayer.inVent,
            DidKill,
            DidSabotage,
            SabotageUsed,
            DidDoors,
            NeuroPlugin.Instance.Impostor.NearbyDoors,
            DoorsUsed
        );
        Frames.Add(frame);

        // string frameString = JsonSerializer.Serialize(frame);
        // Info(frameString);
        ResetFrameData();
    }

    private void ResetFrameData()
    {
        DidReport = DidVent = DidKill = DidSabotage = DidDoors = false;
        SabotageUsed = default;
        DoorsUsed.Clear();
    }

    public void RecordSabotage(SystemTypes type)
    {
        DidSabotage = true;
        SabotageUsed = type;
    }

    public void RecordDoors(SystemTypes room)
    {
        DidDoors = true;
        DoorsUsed = ShipStatus.Instance.AllDoors.Where(r => r.Room == room).ToList();
    }

    [EventHandler(EventTypes.MeetingStarted)]
    public void WriteData()
    {
        string frameString = JsonSerializer.Serialize(Frames);
        File.WriteAllText(Path.Combine(BepInEx.Paths.PluginPath, "output.json"), frameString);
        Info(Path.Combine(BepInEx.Paths.PluginPath, "output.json"));
    }
}
