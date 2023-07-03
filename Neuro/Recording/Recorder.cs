using System;
using System.IO;
using BepInEx;
using Google.Protobuf;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Communication.AmongUsAI;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Recording;

[RegisterInIl2Cpp, ShipStatusComponent]
public sealed class Recorder : MonoBehaviour
{
    public static Recorder Instance { get; private set; }

    public Recorder(IntPtr ptr) : base(ptr)
    {
    }

    private int _fixedUpdateCalls;
    private string _filePath;
    private FileStream _fileStream;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        string recordingsDirectory = Path.Combine(Paths.PluginPath, "NeuroRecordings");
        if (!Directory.Exists(recordingsDirectory)) Directory.CreateDirectory(recordingsDirectory);
        _filePath = Path.Combine(recordingsDirectory, $"{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.gymbag2");
        _fileStream = new FileStream(_filePath, FileMode.Create);

        WriteAndFlush(Frame.Now(true));
        Frame.Cleanup();
    }

    private void FixedUpdate()
    {
        if (MeetingHud.Instance || Minigame.Instance || ExileController.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;

        if (CommunicationHandler.IsPresentAndConnected)
        {
            Warning("Connected to socket, stopping Recorder");
            Destroy(this);
            return;
        }

        _fixedUpdateCalls++;
        if (_fixedUpdateCalls < 5) return;
        _fixedUpdateCalls = 0;

        WriteAndFlush(Frame.Now());
        Frame.Cleanup();
    }

    private void OnDestroy()
    {
        // Ignore small files
        if (_fileStream.Length < 100000) // ~100KB
        {
            Warning("Recording is too small, deleting.");
            _fileStream.Dispose();
            File.Delete(_filePath);
            return;
        }

        _fileStream.Dispose();

        Uploader.Instance.SendFileToServer(Path.GetFileName(_filePath), File.ReadAllBytes(_filePath!));
    }

    [HideFromIl2Cpp]
    private void WriteAndFlush(IMessage message)
    {
        _fileStream.Write(BitConverter.GetBytes(message.CalculateSize()), 0, 4);
        message.WriteTo(_fileStream);
        // Warning($"Recorded: {message}");
        _fileStream.Flush();
    }
}