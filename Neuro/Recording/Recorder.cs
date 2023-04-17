using System;
using System.IO;
using Google.Protobuf;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

// TODO: ReportFindings was removed, we need to implement separate communication with language model
[RegisterInIl2Cpp]
public sealed class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    public Recorder(IntPtr ptr) : base(ptr) { }

    private int _fixedUpdateCalls;
    private FileStream _fileStream;

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

    private void Start()
    {
        string recordingsDirectory = Path.Combine(BepInEx.Paths.PluginPath, "NeuroRecordings");
        if (!Directory.Exists(recordingsDirectory)) Directory.CreateDirectory(recordingsDirectory);
        _fileStream = new FileStream(Path.Combine(recordingsDirectory, $"{DateTime.Now.ToFileTime()}.gymbag2"), FileMode.Create);

        WriteAndFlush(Frame.Now(true));
    }

    private void FixedUpdate()
    {
        // TODO: We should record meeting data!
        if (MeetingHud.Instance || Minigame.Instance) return;

        if (CommunicationHandler.Instance.IsConnected)
        {
            Warning("Connected to socket, stopping Recorder");
            Destroy(this);
            return;
        }

        // TODO: Record local impostor data: kill cooldown, venting stuff, etc
        // TODO: Record local player interactions data: opened task, opened door

        _fixedUpdateCalls++;
        if (_fixedUpdateCalls < 5) return;
        _fixedUpdateCalls = 0;

        Frame frame = Frame.Now();
        Info(frame.Tasks.Tasks);
        Info(frame.LocalPlayer.Velocity);
        WriteAndFlush(frame);
    }

    private void OnDestroy()
    {
        _fileStream.Dispose();
    }

    [HideFromIl2Cpp]
    private void WriteAndFlush(IMessage message)
    {
        _fileStream.Write(BitConverter.GetBytes(message.CalculateSize()), 0, 4);
        message.WriteTo(_fileStream);
        // Warning($"Recorded: {message}");
        _fileStream.Flush();
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<Recorder>();
    }
}
