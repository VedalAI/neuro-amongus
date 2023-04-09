using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Pathfinding;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.Map;

[RegisterInIl2Cpp]
public sealed class MapRecorder : MonoBehaviour, ISerializable
{
    public static MapRecorder Instance { get; private set; }

    public MapRecorder(IntPtr ptr) : base(ptr)
    {
    }

    public List<DoorData> NearbyDoors { get; } = new();
    public List<VentData> NearbyVents { get; } = new();

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(NearbyDoors.Count);
        foreach (DoorData door in NearbyDoors)
            door.Serialize(writer);

        writer.Write(NearbyVents.Count);
        foreach (VentData vent in NearbyVents)
            vent.Serialize(writer);
    }

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
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        UpdateNearbyDoors();
        UpdateNearbyVents();
    }

    private void UpdateNearbyDoors()
    {
        NearbyDoors.Clear();
        foreach (PlainDoor door in ShipStatus.Instance.AllDoors.OrderBy(Closest).Take(3))
        {
            NearbyDoors.Add(DoorData.Create(door));
        }
    }

    private void UpdateNearbyVents()
    {
        NearbyVents.Clear();
        foreach (Vent vent in ShipStatus.Instance.AllVents.OrderBy(Closest).Take(3))
        {
            NearbyVents.Add(VentData.Create(vent));
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
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<MapRecorder>();
    }
}
