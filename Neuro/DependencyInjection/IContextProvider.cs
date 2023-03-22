using Neuro.Vision;

namespace Neuro.DependencyInjection;

public interface IContextProvider
{
    public IVisionHandler VisionHandler { get; }
}
