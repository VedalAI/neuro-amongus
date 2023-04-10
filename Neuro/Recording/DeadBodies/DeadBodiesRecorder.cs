using System;
using System.Linq;
using Neuro.Events;
using Neuro.Utilities;
using Neuro.Vision;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.DeadBodies;

[RegisterInIl2Cpp]
public sealed class DeadBodiesRecorder : MonoBehaviour
{
    public static DeadBodiesRecorder Instance { get; private set; }

    public DeadBodiesRecorder(IntPtr ptr) : base(ptr)
    {
    }

    public DeadBodiesFrame Frame { get; } = new();

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

            if (Frame.DeadBodies.All(d => d.ParentId != deadBody.ParentId))
            {
                Frame.DeadBodies.Add(DeadBodyData.Create(deadBody));
            }
        }
    }

    [EventHandler(EventTypes.MeetingEnded)]
    public void ResetAfterMeeting()
    {
        Frame.DeadBodies.Clear();
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<DeadBodiesRecorder>();
    }
}
