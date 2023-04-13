using System;
using System.IO;
using Google.Protobuf;
using Neuro.Events;
using Neuro.Recording.DeadBodies;
using Neuro.Recording.Header;
using Neuro.Recording.LocalPlayer;
using Neuro.Recording.Map;
using Neuro.Recording.OtherPlayers;
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
        _fileStream = new FileStream(Path.Combine(recordingsDirectory, $"{DateTime.Now.ToFileTime()}.gymbag"), FileMode.Create);
    }

    private void FixedUpdate()
    {
        // TODO: We should record meeting data!
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        // TODO: Record all of the tasks
        // TODO: Record 11th task as emergency
        // TODO: Record localplayer velocity

        _fixedUpdateCalls++;
        if (_fixedUpdateCalls < 9) return;
        _fixedUpdateCalls = 0;

        Serialize(_fileStream);
    }

    private void OnDestroy()
    {
        _fileStream.Dispose();
    }

    public void Serialize(Stream stream)
    {
        Frame frame = new()
        {
            DeadBodies = DeadBodiesRecorder.Instance.Frame,
            Header = HeaderRecorder.GenerateHeaderFrame(),
            LocalPlayer = LocalPlayerRecorder.Instance.Frame,
            Map = MapRecorder.Instance.Frame,
            OtherPlayers = OtherPlayersRecorder.Instance.Frame
        };

        frame.WriteTo(stream);
        // if (stream is MemoryStream) Warning($"Sent: {frame}");
        stream.Flush();
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<Recorder>();
    }
}
