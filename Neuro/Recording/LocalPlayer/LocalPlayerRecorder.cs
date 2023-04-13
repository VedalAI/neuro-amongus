using System;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.LocalPlayer;

[RegisterInIl2Cpp]
public sealed class LocalPlayerRecorder : MonoBehaviour
{
    public static LocalPlayerRecorder Instance { get; private set; }

    public static readonly Vector2[] RaycastDirections =
    {
        Vector2.up,
        Vector2.up + Vector2.right,
        Vector2.right,
        Vector2.right + Vector2.down,
        Vector2.down,
        Vector2.down + Vector2.left,
        Vector2.left,
        Vector2.left + Vector2.up
    };

    public LocalPlayerRecorder(IntPtr ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp]
    public LocalPlayerFrame Frame { get; } = new();

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;

        Frame.RaycastObstacleDistances.FillWithDefault(8);
    }

    private void FixedUpdate()
    {
        Vector2 playerPos = PlayerControl.LocalPlayer.GetTruePosition();

        for (int i = 0; i < 8; i++)
        {
            Physics2D.queriesHitTriggers = false;
            RaycastHit2D raycastHit = Physics2D.Raycast(playerPos, RaycastDirections[i], 100f, Constants.ShipAndAllObjectsMask);
            Physics2D.queriesHitTriggers = true;

            Frame.RaycastObstacleDistances[i] = raycastHit.distance;
        }
    }

    public void RecordReport() => Frame.DidReport = true;
    public void RecordVent() => Frame.DidVent = true;
    public void RecordKill() => Frame.DidKill = true;
    public void RecordSabotage(SystemTypes type) => Frame.SabotageUsed = (byte) type;
    public void RecordDoors(SystemTypes room) => Frame.DoorsUsed = (byte) room;

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<LocalPlayerRecorder>();
    }
}
