using System.Threading;
using Il2CppInterop.Runtime;

namespace Neuro.Utilities;

public abstract class NeuroThread
{
    protected Thread _thread { get; private set; }

    protected NeuroThread()
    {
        _thread = new Thread(() =>
        {
            IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
            RunThread();
        });
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
        if (_thread.IsAlive)
        {
            _thread.Interrupt();
        }
    }

    protected abstract void RunThread();
}
