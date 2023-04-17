using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Neuro.Utilities;

namespace Neuro.Communication.AmongUsAI;

public sealed class WebSocketThread : NeuroThread
{
    private static readonly IPEndPoint _endPoint = new(IPAddress.Parse("127.0.0.1"), 6969);

    public Socket Socket { get; private set; }
    public event Action OnConnect = delegate { };

    // This method will try connecting every 5s, for as long as the thread is not interrupted.
    protected override async void RunThread()
    {
        while (true)
        {
            try
            {
                await Task.Delay(5000, CancellationToken);

                if (Socket == null || (Socket.Poll(1000, SelectMode.SelectRead) && Socket.Available == 0) || !Socket.Connected)
                {
                    Warning("Connecting to socket");
                    Socket?.Close();
                    Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await Socket.ConnectAsync(_endPoint, CancellationToken);
                    OnConnect?.Invoke();
                }
            }
            catch (OperationCanceledException)
            {
                Socket?.Shutdown(SocketShutdown.Both);
                Socket?.Close();
                return;
            }
            catch (SocketException e)
            {
                Error(e.Message);
            }
            catch (Exception e)
            {
                Error(e);
            }
        }
    }
}
