using System;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Caching;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.DeadBodies;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class DeadBodiesRecorder : MonoBehaviour // TODO: add dead bodies seen on cams
{
    public static DeadBodiesRecorder Instance { get; private set; }

    public DeadBodiesRecorder(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp] public DeadBodiesFrame Frame { get; } = new();

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        EventManager.RegisterHandler(this);
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;

        Collider2D closestActive = null;
        foreach (Collider2D collider2D in Physics2D.OverlapCircleAll(PlayerControl.LocalPlayer.GetTruePosition(), PlayerControl.LocalPlayer.MaxReportDistance, Constants.PlayersOnlyMask))
        {
            if (collider2D.tag == "DeadBody")
            {
                closestActive = collider2D;
                break;
            }
        }

        foreach (DeadBody deadBody in ComponentCache<DeadBody>.Cached)
        {
            if (Frame.DeadBodies.All(d => d.ParentId != deadBody.ParentId))
            {
                if (Visibility.IsVisible(deadBody.TruePosition) ||
                    (HudManager.Instance.ReportButton.canInteract && deadBody.myCollider.GetInstanceID() == closestActive!?.GetInstanceID()))
                {
                    Frame.DeadBodies.Add(DeadBodyData.Create(deadBody));
                }
            }
        }
    }

    [EventHandler(EventTypes.MeetingEnded)]
    public void ResetAfterMeeting()
    {
        Frame.DeadBodies.Clear();
    }
}
