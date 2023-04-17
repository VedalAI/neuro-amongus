using System.Threading;
using Il2CppInterop.Runtime;

namespace Neuro.Utilities;

public abstract class NeuroThread
{
    private readonly Thread _thread;
    private readonly CancellationTokenSource _cancellationTokenSource;

    protected CancellationToken CancellationToken => _cancellationTokenSource.Token;

    protected NeuroThread()
    {
        _thread = new Thread(() =>
        {
            Il2CppAttach();
            RunThread();
        });
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Start()
    {
        if (!_thread.IsAlive)
        {
            _thread.Start();
        }
    }

    public void Stop()
    {
        if (_thread.IsAlive && !_cancellationTokenSource.IsCancellationRequested)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    protected static void Il2CppAttach() => IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());

    protected abstract void RunThread();
}
