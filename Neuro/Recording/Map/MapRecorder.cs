using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Pathfinding;
using Neuro.Recording.Common;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.Map;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class MapRecorder : MonoBehaviour
{
    public static MapRecorder Instance { get; private set; }

    public MapRecorder(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp] public MapFrame Frame { get; } = new();

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

        UpdateNearbyDoors();
        UpdateNearbyVents();
        UpdateMeetingButton();
    }

    private void UpdateNearbyDoors()
    {
        Frame.NearbyDoors.Clear();
        foreach (PlainDoor door in ShipStatus.Instance.AllDoors.OrderBy(Closest).Take(3))
        {
            Frame.NearbyDoors.Add(DoorData.Create(door));
        }
    }

    private void UpdateNearbyVents()
    {
        Frame.NearbyVents.Clear();
        foreach (Vent vent in ShipStatus.Instance.AllVents.OrderBy(Closest).Take(3))
        {
            Frame.NearbyVents.Add(VentData.Create(vent));
        }
    }

    private void UpdateMeetingButton()
    {
        Frame.MeetingButton = PositionData.Create(ShipStatus.Instance.EmergencyButton);
    }

    private float Closest(PlainDoor door)
    {
        return PathfindingHandler.Instance.GetPathLength(door);
    }

    private float Closest(Vent vent)
    {
        return PathfindingHandler.Instance.GetPathLength(vent);
    }
}