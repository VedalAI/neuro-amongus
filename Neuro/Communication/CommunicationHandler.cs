﻿using System;
using System.Text;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Reactor.Utilities.Attributes;
using UnityEngine;
using System.Text.Json;
using Neuro.Recording.DataStructures;

namespace Neuro.Communcation;

[RegisterInIl2Cpp]
public sealed class CommunicationHandler : MonoBehaviour
{
    public CommunicationHandler(IntPtr ptr) : base(ptr) { }

    IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6969);
    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    bool hasGotResponse = true;


    private void Start()
    {
        socket.Connect(iPEndPoint);
    }


    private void FixedUpdate()
    {
        if (!ShipStatus.Instance) return;
        if (MeetingHud.Instance) return;
        if (Minigame.Instance) return;
        if (!PlayerControl.LocalPlayer) return;

        if (socket.Available > 0)
        {
            var buffer = new byte[1_024];
            var received = socket.Receive(buffer, SocketFlags.None);
            var response = Encoding.UTF8.GetString(buffer, 0, received);
            Info(response);
            Frame frame = JsonSerializer.Deserialize<Frame>(response);
            Info(frame);
            NeuroPlugin.Instance.Executor.ProcessFrame(frame);
            hasGotResponse = true;
        }


        if (hasGotResponse)
        {
            socket.Send(Encoding.ASCII.GetBytes(JsonSerializer.Serialize<Frame>(NeuroPlugin.Instance.Recording.Frames.Last())));
            hasGotResponse = false;
        }
    }
}
