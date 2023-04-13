﻿using Il2CppInterop.Runtime.Attributes;
using Il2CppInterop.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Epic.OnlineServices.Auth;
using System.Threading;
using Rewired.Utils.Classes.Utility;
using SelectMode = System.Net.Sockets.SelectMode;

namespace Neuro.Communication.AmongUsAI;

public sealed class WebSocketThread
{

    private static readonly IPEndPoint _ipEndPoint = new(IPAddress.Parse("127.0.0.1"), 6969);
    internal static Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    private static void GracefulSocketRestart() {
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Disconnect(false); //reusing sockets for the same client is impossible.
        _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    [HideFromIl2Cpp]
    //This method will try connecting every 5s, for as long as the component is enabled.
    //This should retain the socket connection even if the world blows up and only the client is running.
    internal static void ConnectToServer(object state)
    {
        bool _require_startup = true;
        try
        {
            var token = (CancellationToken)state;
            IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());

            while (true)
            {
                // Check if the cancellation is requested
                if (token.IsCancellationRequested)
                {
                    GracefulSocketRestart();
                    break;
                }
                bool isConnected = !(_socket.Poll(1000, SelectMode.SelectRead) && _socket.Available == 0);
                if (!isConnected || _require_startup)
                {
                    Info("Trying connection to python server..");
                    try
                    {
                        if (_socket.Connected)
                        {
                            //handle ungraceful disconnects
                            GracefulSocketRestart();
                        }
                        _socket.Connect(_ipEndPoint);
                    }
                    catch (Exception ex) {
                        //very spammy if connections fail a lot.
                       //Warning(ex);
                    }
                    if (!_socket.Connected)
                    {
                        Warning("Failed connecting to python server.");
                        _require_startup = true;
                    }
                    else {
                        _require_startup = false;
                        Info("Socket connected.");
                    }
                }
                // Wait for 5 seconds before trying again
                Thread.Sleep(5000);
                //Info("Thread heartbeat");
            }
        }
        catch (ThreadInterruptedException)
        {
            return;
        }
        catch (Exception e)
        {
            Warning(e);
            try
            {
                Thread.Yield();
            }
            catch
            {
                return;
            }
        }
    }
}