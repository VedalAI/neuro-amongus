using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Neuro.DependencyInjection;
using Neuro.Recording.DataStructures;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

[RegisterInIl2Cpp]
public class RecordingHandler : MonoBehaviour, IRecordingHandler
{
    public RecordingHandler(IntPtr ptr) : base(ptr) { }

    public IContextProvider Context { get; set; }

    public List<Frame> Frames { get; set; } = new();

    public void FixedUpdate()
    {
        // Record values
        Frame frame = new(
            PlayerControl.LocalPlayer.Data.Role.IsImpostor,
            PlayerControl.LocalPlayer.killTimer,
            directionToNearestTask,
            PlayerControl.LocalPlayer.myTasks.ToArray().Any(PlayerTask.TaskIsEmergency),
            Vector2.zero,
            Context.VisionHandler.DirectionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.gameObject.activeInHierarchy, // TODO: Check if this is correct
            new List<PlayerRecord>(),
            moveDirection,
            false,
            false,
            false,
            false,
            false
        );
        string frameString = JsonSerializer.Serialize(frame);
        Debug.Log(frameString);
        Frames.Add(frame);
    }
}
