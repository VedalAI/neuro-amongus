using System;
using System.Collections.Generic;
using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Neuro.Recording.DeadBodies;
using Neuro.Recording.Impostor;
using Neuro.Recording.LocalPlayer;
using Neuro.Recording.Map;
using Neuro.Recording.OtherPlayers;
using Neuro.Utilities;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

// TODO: ReportFindings was removed, we need to implement separate communication with language model
[RegisterInIl2Cpp]
public sealed class Recorder : MonoBehaviour, ISerializable
{
    public static Recorder Instance { get; private set; }

    public Recorder(IntPtr ptr) : base(ptr) { }

    private List<ISerializable> _recorders = new();
    private int _fixedUpdateCalls = 0;
    private FileStream _fileStream;
    private BinaryWriter _fileWriter;

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
        _recorders = new List<ISerializable>
        {
            DeadBodiesRecorder.Instance,
            ImpostorRecorder.Instance,
            LocalPlayerRecorder.Instance,
            MapRecorder.Instance,
            OtherPlayersRecorder.Instance
        };

        _fileStream = new FileStream(Path.Combine(BepInEx.Paths.PluginPath, "recording.gymbag"), FileMode.CreateNew);
        _fileWriter = new BinaryWriter(_fileStream);
    }

    private void FixedUpdate()
    {
        // TODO: We should record meeting data!
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        // TODO: Record map id
        // TODO: Record all of the tasks
        // TODO: Record 11th task as emergency
        // TODO: Record fellow impostors
        // TODO: Record localplayer velocity
        // TODO: Raycast for obstacles

        _fixedUpdateCalls++;
        if (_fixedUpdateCalls < 9) return;
        _fixedUpdateCalls = 0;

        Serialize(_fileWriter);
    }

    private void OnDestroy()
    {
        _fileWriter.Dispose();
        _fileStream.Dispose();
    }

    public void Serialize(BinaryWriter writer)
    {
        foreach (ISerializable recorder in _recorders)
        {
            recorder.Serialize(_fileWriter);
        }
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<Recorder>();
    }
}
