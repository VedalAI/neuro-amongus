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
    private volatile bool _needsHeaderFrame;
    private bool _shouldSend = true;

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
        _thread = new WebSocketThread();
        _thread.OnConnect += () => _needsHeaderFrame = true;
        _thread.Start();
    }

    private void OnDestroy()
    {
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
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

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
            Send(Frame.Now(_needsHeaderFrame));
            _needsHeaderFrame = false;
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
        MovementHandler.Instance.ForcedMoveDirection = output.DesiredMoveDirection;
    }
}
