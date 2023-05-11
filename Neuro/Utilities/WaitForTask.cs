using System;
using System.Collections;
using System.Threading.Tasks;

namespace Neuro.Utilities;

public class WaitForTask : IEnumerator
{
    private readonly Task _task;

    object IEnumerator.Current => null;

    public WaitForTask(Task task)
    {
        _task = task ?? throw new ArgumentNullException(nameof(task));
    }

    bool IEnumerator.MoveNext()
    {
        return !_task.IsCompleted;
    }

    void IEnumerator.Reset()
    {
        throw new NotSupportedException();
    }
}
