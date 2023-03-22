namespace Neuro.DependencyInjection;

public interface IContextAcceptor
{
    public IContextProvider Context { set; }
}
