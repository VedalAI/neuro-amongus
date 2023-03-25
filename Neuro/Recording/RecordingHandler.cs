using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Neuro.Recording.DataStructures;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

[RegisterInIl2Cpp]
public class RecordingHandler : MonoBehaviour
{
    public RecordingHandler(IntPtr ptr) : base(ptr) { }

    public List<Frame> Frames { get; set; } = new();

    // TODO: Maybe there's a better way to store this than to keep random properties in this class
    public bool DidReport { get; set; }
    public bool DidVent { get; set; }
    public bool DidKill { get; set; }

    public void FixedUpdate()
    {
        if (MeetingHud.Instance) return;

        // Record values
        Frame frame = new(
            PlayerControl.LocalPlayer.Data.Role.IsImpostor,
            PlayerControl.LocalPlayer.killTimer,
            NeuroPlugin.Instance.MovementHandler.DirectionToNearestTask,
            PlayerControl.LocalPlayer.myTasks.ToArray().Any(PlayerTask.TaskIsEmergency),
            Vector2.zero,
            NeuroPlugin.Instance.VisionHandler.DirectionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.isActiveAndEnabled,
            new List<PlayerRecord>(),
            NeuroPlugin.Instance.MovementHandler.LastMoveDirection,
            DidReport,
            DidVent,
            DidKill,
            // TODO: Implement these two
            false,
            false
        );
        string frameString = JsonSerializer.Serialize(frame);
        Info(frameString);
        Frames.Add(frame);

        DidReport = DidVent = DidKill = false;
    }
}
