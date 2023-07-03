using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Neuro.Threading;

namespace Neuro.Communication.AmongUsAI;

public sealed class WebSocketThread : ParallelThreadWorker
{
    private static readonly IPEndPoint _endPoint = new(IPAddress.Parse("127.0.0.1"), 6969);

    public Socket Socket { get; private set; }
    public event Action OnConnect = delegate { };

    protected override void RunThread()
    {
        while (true)
        {
            try
            {
                Thread.Sleep(5000);

                if (!CommunicationHandler.Instance.enabled) continue;

                if (Socket == null || (Socket.Poll(1000, SelectMode.SelectRead) && Socket.Available == 0) || !Socket.Connected)
                {
                    Warning("Connecting to socket");
                    Socket?.Close();
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    Socket.Connect(_endPoint);
                    OnConnect?.Invoke();
                }
            }
            catch (ThreadInterruptedException)
            {
                System.Console.WriteLine("[WEBSOCKET] Caught thread interrupted");
                // Socket?.Shutdown(SocketShutdown.Both);
                // Socket?.Close();
                return;
            }
            catch (SocketException e)
            {
                System.Console.WriteLine("[WEBSOCKET] Caught SOCKET exception");
                Error(e.Message);
            }
            catch (Exception e)
            {
                System.Console.WriteLine("[WEBSOCKET] Caught another exception: " + e);
                Error(e);
            }
        }
    }
}