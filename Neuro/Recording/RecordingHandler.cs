using System;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Recording.DataStructures;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

[RegisterInIl2Cpp]
public class RecordingHandler : MonoBehaviour
{
    public RecordingHandler(IntPtr ptr) : base(ptr) { }

    [HideFromIl2Cpp]
    public List<Frame> Frames { get; set; } = new();

    // TODO: Maybe there's a better way to store this than to keep random properties in this class
    public bool DidReport { get; set; }
    public bool DidVent { get; set; }
    public bool DidKill { get; set; }

    public void FixedUpdate()
    {
        if (!ShipStatus.Instance) return;
        if (MeetingHud.Instance) return;
        if (!PlayerControl.LocalPlayer) return;

        // Record values
        Frame frame = new(
            PlayerControl.LocalPlayer.Data.Role.IsImpostor,
            PlayerControl.LocalPlayer.killTimer,
            NeuroPlugin.Instance.Movement.DirectionToNearestTask,
            PlayerControl.LocalPlayer.myTasks.ToArray().Any(PlayerTask.TaskIsEmergency),
            Vector2.zero,
            NeuroPlugin.Instance.Vision.DirectionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.isActiveAndEnabled,
            NeuroPlugin.Instance.Vision.PlayerRecords,
            NeuroPlugin.Instance.Movement.LastMoveDirection,
            DidReport,
            DidVent,
            DidKill,
            // TODO: Implement these two
            false,
            false
        );
        Frames.Add(frame);

        // string frameString = JsonSerializer.Serialize(frame);
        // Info(frameString);

        DidReport = DidVent = DidKill = false;
    }
}
