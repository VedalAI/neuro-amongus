using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
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
            string response = Encoding.UTF8.GetString(_buffer, 0, received);
            Info(response);

            // TODO: Deserialize data

            // Frame frame = JsonSerializer.Deserialize<Frame>(response);
            // Info(frame);
            // NeuroPlugin.Instance.Movement.ForcedMoveDirection = new Vector2(frame.Direction.x, frame.Direction.y).normalized;

            _hasGotResponse = true;
        }

        if (_hasGotResponse)
        {
            // TODO: Serialize data for sending over socket
            // _socket.Send(Encoding.ASCII.GetBytes(JsonSerializer.Serialize<Frame>(NeuroPlugin.Instance.Recording.Frames.Last())));
            _hasGotResponse = false;
        }
    }
}
