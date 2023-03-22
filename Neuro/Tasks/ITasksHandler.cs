using System.Collections;
using Neuro.DependencyInjection;

namespace Neuro.Tasks;

public interface ITasksHandler : IContextAcceptor
{
    public IEnumerator EvaluatePath(NormalPlayerTask task);
}
