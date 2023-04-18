using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Pathfinding;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.Map;

[RegisterInIl2Cpp]
public sealed class MapRecorder : MonoBehaviour
{
    public static MapRecorder Instance { get; private set; }

    public MapRecorder(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp]
    public MapFrame Frame { get; } = new();

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance) return;

        UpdateNearbyDoors();
        UpdateNearbyVents();
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

    private float Closest(PlainDoor door)
    {
        return PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, door, door);
    }

    private float Closest(Vent vent)
    {
        return PathfindingHandler.Instance.GetPathLength(PlayerControl.LocalPlayer, vent, vent);
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<MapRecorder>();
    }
}
