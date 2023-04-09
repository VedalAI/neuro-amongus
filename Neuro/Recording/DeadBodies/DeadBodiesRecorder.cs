using System;
using System.Collections.Generic;
using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Utilities;
using Neuro.Vision;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.DeadBodies;

[RegisterInIl2Cpp]
public sealed class DeadBodiesRecorder : MonoBehaviour, ISerializable
{
    public static DeadBodiesRecorder Instance { get; private set; }

    public DeadBodiesRecorder(IntPtr ptr) : base(ptr)
    {
    }

    private Dictionary<byte, DeadBodyData> SeenBodies { get; } = new();

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(SeenBodies.Count);
        foreach (DeadBodyData body in SeenBodies.Values)
        {
            body.Serialize(writer);
        }
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
        EventManager.RegisterHandler(this);
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance) return;

        foreach (DeadBody deadBody in ComponentCache<DeadBody>.Cached)
        {
            if (!Visibility.IsVisible(deadBody)) continue;

            if (!SeenBodies.ContainsKey(deadBody.ParentId))
            {
                SeenBodies[deadBody.ParentId] = DeadBodyData.Create(deadBody);
            }
        }
    }

    [EventHandler(EventTypes.MeetingEnded)]
    public void ResetAfterMeeting()
    {
        SeenBodies.Clear();
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<DeadBodiesRecorder>();
    }
}
