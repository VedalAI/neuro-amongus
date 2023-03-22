using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.Utils;
using Reactor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Neuro;

// [BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public class NeuroPlugind : BasePlugin
{
    public LineRenderer arrow;


    public Vector2 directionToNearestTask;
    public Vector2 moveDirection;

    public Recorder recorder = new();

    public Vision vision = new();

    public void FixedUpdate()
    {
        // TODO: This method should be split into multiple MonoBehaviours - one for vision, one for recording and one for doing tasks

        if (MeetingHud.Instance) return;

        vision.UpdateVision();

        // Record values
        Frame frame = new(
            PlayerControl.LocalPlayer.Data.Role.IsImpostor,
            PlayerControl.LocalPlayer.killTimer,
            directionToNearestTask,
            PlayerControl.LocalPlayer.myTasks.ToArray().Any(PlayerTask.TaskIsEmergency),
            Vector2.zero,
            vision.directionToNearestBody,
            GameManager.Instance.CanReportBodies() && HudManager.Instance.ReportButton.gameObject.activeInHierarchy,
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
        recorder.Frames.Add(frame);
    }

    public void ShipStatus_Awake()
    {
        GameObject arrowObject = new("Arrow");
        arrow = arrowObject.AddComponent<LineRenderer>();
    }

    public bool MovePlayer(ref Vector2 direction)
    {
        // TODO: Move to separate MonoBehaviour for handling movement

        moveDirection = direction;
        if (currentPath.Length > 0 && pathIndex != -1)
        {
            Vector2 nextWaypoint = currentPath[pathIndex];

            while (Vector2.Distance(PlayerControl.LocalPlayer.GetTruePosition(), nextWaypoint) < 0.75f)
            {
                pathIndex++;
                if (pathIndex > currentPath.Length - 1)
                {
                    pathIndex = currentPath.Length - 1;
                    nextWaypoint = currentPath[pathIndex];
                    break;
                }

                nextWaypoint = currentPath[pathIndex];
            }

            directionToNearestTask = (nextWaypoint - PlayerControl.LocalPlayer.GetTruePosition()).normalized;

            LineRenderer renderer = arrow;
            renderer.SetPosition(0, PlayerControl.LocalPlayer.GetTruePosition());
            renderer.SetPosition(1, PlayerControl.LocalPlayer.GetTruePosition() + directionToNearestTask);
            renderer.widthMultiplier = 0.1f;
            renderer.positionCount = 2;
            renderer.startColor = Color.red;
        }
        else
        {
            directionToNearestTask = Vector2.zero;
        }

        return true;
    }
}
