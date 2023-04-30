using System.Collections.Generic;
using System.Threading;
using Il2CppInterop.Runtime;

namespace Neuro.Utilities;

public abstract class NeuroThread
{
    private readonly List<Thread> _threads = new();

    protected NeuroThread(int count = 1)
    {
        for (int i = 0; i < count; i++)
        {
            int id = i;
            _threads.Add(new Thread(() =>
            {
                Thread.BeginThreadAffinity();
                IL2CPP.il2cpp_thread_attach(IL2CPP.il2cpp_domain_get());
                RunThread(id);
                Thread.EndThreadAffinity();
            }));
        }
    }

    public void Start()
    {
        foreach (Thread thread in _threads)
        {
            if (!thread.IsAlive)
            {
                thread.Start();
            }
        }
    }

    public void Stop()
    {
        foreach (Thread thread in _threads)
        {
            thread.Interrupt();
        }
    }

    protected abstract void RunThread(int id);
}
