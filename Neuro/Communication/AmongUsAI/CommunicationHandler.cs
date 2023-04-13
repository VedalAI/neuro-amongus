using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Google.Protobuf;
using Il2CppInterop.Runtime.Attributes;
using System.Threading;
using Neuro.Events;
using Neuro.Movement;
using Neuro.Recording;
using Reactor.Utilities.Attributes;
using UnityEngine;
using Neuro.Recording.Header;

namespace Neuro.Communication.AmongUsAI;

[RegisterInIl2Cpp]
public sealed class CommunicationHandler : MonoBehaviour
{
    public CommunicationHandler(IntPtr ptr) : base(ptr) { }

    private CancellationTokenSource cts;
    private Thread _thread;

    private void Start()
    {
        //graceful start
        if (_thread == null || !_thread.IsAlive)
        {
            cts = new CancellationTokenSource();
            _thread = new Thread(new ParameterizedThreadStart(WebSocketThread.ConnectToServer));
            _thread.Start(cts.Token);
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
    }

    private readonly byte[] _buffer = new byte[1024];
    private bool _hasGotResponse = true;
    internal static volatile bool needsHeaderFrame = true;

    private void FixedUpdate()
    {
        // TODO: We should send meeting data!

        if (!WebSocketThread._socket.Connected) {
            //send data on connect, incase of reset.
            _hasGotResponse = true;
            return;
        }
        if (MeetingHud.Instance || Minigame.Instance || !PlayerControl.LocalPlayer) return;

        if (WebSocketThread._socket.Available > 0)
        {
            int received = WebSocketThread._socket.Receive(_buffer, SocketFlags.None);
            NNOutput output = NNOutput.Parser.ParseFrom(_buffer, 0, received);
            // Warning($"Received: {output}");
            HandleOutput(output);

            _hasGotResponse = true;
        }

        if (_hasGotResponse)
        {
            if (needsHeaderFrame)
            {
                Send(HeaderFrame.Generate());
            }
            else {
                Send(Frame.Now);
            }
            
            // Warning($"Sent: {Frame.Now}");

            _hasGotResponse = false;
        }
    }

    [HideFromIl2Cpp]
    private void Send(IMessage message)
    {
        using MemoryStream stream = new();
        message.WriteTo(stream);
        WebSocketThread._socket.Send(stream.ToArray());
    }

    [HideFromIl2Cpp]
    private void HandleOutput(NNOutput output)
    {
        MovementHandler.Instance.ForcedMoveDirection = output.DesiredMoveDirection;
    }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        ShipStatus.Instance.gameObject.AddComponent<CommunicationHandler>();
    }
}
