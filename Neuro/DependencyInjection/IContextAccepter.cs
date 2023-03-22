namespace Neuro.DependencyInjection;

public interface IContextAccepter
{
    public IContextProvider Context { set; }
}
