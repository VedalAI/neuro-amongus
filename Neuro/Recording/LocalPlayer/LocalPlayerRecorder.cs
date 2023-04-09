using System;
using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording.LocalPlayer;

[RegisterInIl2Cpp]
public sealed class LocalPlayerRecorder : MonoBehaviour, ISerializable
{
    public static LocalPlayerRecorder Instance { get; private set; }

    public LocalPlayerRecorder(IntPtr ptr) : base(ptr)
    {
    }

    private bool DidReport { get; set; }
    private bool DidVent { get; set; }
    private bool DidKill { get; set; }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(DidReport);
        writer.Write(DidVent);
        writer.Write(DidKill);

        DidReport = DidVent = DidKill = false;
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

    public void RecordReport() => DidReport = true;
    public void RecordVent() => DidVent = true;
    public void RecordKill() => DidKill = true;

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<LocalPlayerRecorder>();
    }
}
