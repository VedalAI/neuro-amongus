using System;
using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.Impostor;

[RegisterInIl2Cpp]
public sealed class ImpostorRecorder : MonoBehaviour, ISerializable
{
    public static ImpostorRecorder Instance { get; private set; }

    public ImpostorRecorder(IntPtr ptr) : base(ptr)
    {
    }

    public bool DidKill { get; private set; }
    public int SabotageUsed { get; private set; } = -1;
    public int DoorsUsed { get; private set; } = -1;

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(DidKill);
        writer.Write(SabotageUsed);
        writer.Write(DoorsUsed);

        DidKill = false;
        SabotageUsed = DoorsUsed = -1;
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

    public void RecordKill() => DidKill = true;
    public void RecordSabotage(SystemTypes type) => SabotageUsed = (int) type;
    public void RecordDoors(SystemTypes room) => DoorsUsed = (int) room;

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<ImpostorRecorder>();
    }
}
