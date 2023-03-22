using Neuro.DependencyInjection;

namespace Neuro.Vision;

public interface IVisionHandler : IContextAcceptor
{
    public void Update();

    public void ReportFindings();
}
