using System;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.LocalPlayer;

[RegisterInIl2Cpp]
public sealed class LocalPlayerRecorder : MonoBehaviour
{
    public static LocalPlayerRecorder Instance { get; private set; }

    public LocalPlayerRecorder(IntPtr ptr) : base(ptr)
    {
    }

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
    }

    public void RecordReport() => Frame.DidReport = true;
    public void RecordVent() => Frame.DidVent = true;
    public void RecordKill() => Frame.DidKill = true;
    public void RecordSabotage(SystemTypes type) => Frame.SabotageUsed = (byte) type;
    public void RecordDoors(SystemTypes room) => Frame.DoorsUsed = (byte) room;

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<LocalPlayerRecorder>();
    }
}
