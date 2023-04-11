using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Neuro.Events;
using Neuro.Movement;
using Neuro.Recording;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Communication.AmongUsAI;

[RegisterInIl2Cpp]
public sealed class CommunicationHandler : MonoBehaviour
{
    public CommunicationHandler(IntPtr ptr) : base(ptr) { }

    private static readonly IPEndPoint _ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 6969);
    private static readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private void Start()
    {
        _socket.Connect(_ipEndPoint);
    }

    private readonly byte[] _buffer = new byte[1024];
    private bool _hasGotResponse = true;

    public void FixedUpdate()
    {
        // TODO: We should send meeting data!

        if (!_socket.Connected) return;
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        if (_socket.Available > 0)
        {
            int received = _socket.Receive(_buffer, SocketFlags.None);
            NNOutput output = NNOutput.Parser.ParseFrom(_buffer, 0, received);
            // Warning($"Received: {output}");
            HandleOutput(output);

            _hasGotResponse = true;
        }

        if (_hasGotResponse)
        {
            using MemoryStream memoryStream = new();
            Recorder.Instance.Serialize(memoryStream);
            _socket.Send(memoryStream.ToArray());

            _hasGotResponse = false;
        }
    }

    private void HandleOutput(NNOutput output)
    {
        MovementHandler.Instance.ForcedMoveDirection = output.DesiredMoveDirection;
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted(ShipStatus shipStatus)
    {
        shipStatus.gameObject.AddComponent<CommunicationHandler>();
    }
}
