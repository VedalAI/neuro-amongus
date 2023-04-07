using System;
using System.Collections.Generic;
using Neuro.Utilities;
using Neuro.Utilities.Caching;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Vision.DeadBodies;

[RegisterInIl2Cpp]
public sealed class DeadBodyVisionHandler : MonoBehaviour
{
    public static DeadBodyVisionHandler Instance { get; private set; }

    public DeadBodyVisionHandler(IntPtr ptr) : base(ptr) { }

    public DeadBody Nearest { get; private set; }

    public Dictionary<byte, DeadBodyVisionData> Data { get; } = new();

    private void Awake()
    {
        if (Instance)
        {
            LogUtils.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance) return;

        Nearest = null;
        float nearestBodyDistance = Mathf.Infinity;

        foreach (DeadBody deadBody in ComponentCache<DeadBody>.Cached)
        {
            if (!Visibility.IsVisible(deadBody)) continue;

            float distance = Vector2.Distance(deadBody.TruePosition, PlayerControl.LocalPlayer.GetTruePosition());
            if (distance < nearestBodyDistance)
            {
                nearestBodyDistance = distance;
                Nearest = deadBody;
            }

            if (!Data.ContainsKey(deadBody.ParentId))
            {
                Data[deadBody.ParentId] = DeadBodyVisionData.Create(deadBody);
            }
        }
    }

    public void ResetAfterMeeting()
    {
        Data.Clear();
    }
}
