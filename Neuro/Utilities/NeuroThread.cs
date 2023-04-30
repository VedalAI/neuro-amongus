using System.Threading;
using Il2CppInterop.Runtime;

namespace Neuro.Utilities;

public abstract class NeuroThread
{
    private readonly Thread _thread;

    protected NeuroThread()
    {
        _thread = new Thread(() =>
        {
            Thread.BeginThreadAffinity();
            IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
            RunThread();
            Thread.EndThreadAffinity();
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
        _thread.Interrupt();
    }

    protected abstract void RunThread();
}
