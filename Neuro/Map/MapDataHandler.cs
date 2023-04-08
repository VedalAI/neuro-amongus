using System;
using System.Collections.Generic;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Map;

[RegisterInIl2Cpp]
public sealed class MapDataHandler : MonoBehaviour
{
    public static MapDataHandler Instance { get; private set; }

    public MapDataHandler(IntPtr ptr) : base(ptr)
    {
    }

    public List<PlainDoor> NearbyDoors { get; } = new();

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
        if (MeetingHud.Instance || Minigame.Instance) return;

        UpdateNearbyDoors();
        UpdateNearbyVents();
    }

    private void UpdateNearbyDoors()
    {
        NearbyDoors.Clear();
        foreach (PlainDoor door in ShipStatus.Instance.AllDoors)
        {
            float distance = Vector2.Distance(door.transform.position, PlayerControl.LocalPlayer.GetTruePosition());
            if (distance < 10f)
            {
                NearbyDoors.Add(door);
            }
        }
    }

    private void UpdateNearbyVents()
    {
        Vent closest = null;
        float closestDistance = 999f;
        NearbyVents.Clear();
        foreach (Vent vent in ShipStatus.Instance.AllVents)
        {
            float distance = Vector2.Distance(vent.transform.position, PlayerControl.LocalPlayer.transform.position);
            if (distance < 10f)
            {
                NearbyVents.Add(vent);
                // also take the opportunity to get the closest vent
                if (distance < closestDistance)
                {
                    closest = vent;
                    closestDistance = distance;
                }
            }
        }
        ClosestVent = closest;

        if (ClosestVent == null)
        {
            DirectionToNearestVent = Vector2.zero;
        }
        else
        {
            DirectionToNearestVent = (ClosestVent.transform.position - PlayerControl.LocalPlayer.transform.position).normalized;
        }

    }

    [EventHandler(EventTypes.GameStarted)]
    public static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<MapDataHandler>();
    }
}
