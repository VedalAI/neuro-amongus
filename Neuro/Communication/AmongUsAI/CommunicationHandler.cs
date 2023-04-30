using System;
using System.IO;
using System.Net.Sockets;
using Google.Protobuf;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Movement;
using Neuro.Recording;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Neuro.Utilities;

namespace Neuro.Communication.AmongUsAI;

[RegisterInIl2Cpp]
public sealed class CommunicationHandler : MonoBehaviour
{
    public static CommunicationHandler Instance { get; private set; }

    public CommunicationHandler(IntPtr ptr) : base(ptr) { }

    private readonly byte[] _buffer = new byte[1024];
    private WebSocketThread _thread;
    private volatile bool _shouldSendHeader = true;
    private bool _shouldSend = true;

    public bool IsConnected => _thread.Socket?.Connected ?? false;

    private void Awake()
    {
        if (Instance)
        {
            NeuroUtilities.WarnDoubleSingletonInstance();
            Destroy(this);
            return;
        }

        Instance = this;

        _thread = new WebSocketThread();
        _thread.OnConnect += () => _shouldSendHeader = true;
    }

    private void Start()
    {
        _thread.Start();
    }

    private void OnApplicationQuit()
    {
        System.Console.WriteLine("[WEBSOCKET] Trying to shut down...");
        _thread.Stop();
    }

    private void FixedUpdate()
    {
        if (_thread.Socket is not {Connected: true})
        {
            // Send data when next connected, in case of reset.
            _shouldSend = true;
            return;
        }

        // TODO: We should send meeting data!
        if (MeetingHud.Instance || Minigame.Instance || !Frame.CanGenerate) return;

        if (PlayerControl.LocalPlayer.Data.IsDead)
        {
            DeadMovementHandler.Instance.Move();
            return;
        }

        if (_thread.Socket.Available > 0)
        {
            int received = _thread.Socket.Receive(_buffer, SocketFlags.None);
            NNOutput output = NNOutput.Parser.ParseFrom(_buffer, 0, received);
            // Warning($"Received: {output}");
            HandleOutput(output);

            _shouldSend = true;
        }

        if (_shouldSend)
        {
            Send(Frame.Now(_shouldSendHeader));
            _shouldSendHeader = false;
            // Warning($"Sent: {Frame.Now}");

            _shouldSend = false;
        }
    }

    [HideFromIl2Cpp]
    private void Send(IMessage message)
    {
        using MemoryStream stream = new();
        message.WriteTo(stream);
        _thread.Socket.Send(stream.ToArray());
    }

    [HideFromIl2Cpp]
    private void HandleOutput(NNOutput output)
    {
        // Info(output);
        MovementHandler.Instance.ForcedMoveDirection = output.DesiredMoveDirection;
    }
}
