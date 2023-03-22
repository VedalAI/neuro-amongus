using Neuro.DependencyInjection;

namespace Neuro.Vision;

public interface IVisionHandler : IContextAcceptor
{
    public void ReportFindings();

    public void ResetAfterMeeting();
}
