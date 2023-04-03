namespace Neuro.Debugging;

public abstract class DebugTab
{
    public abstract string Name { get; }
    public virtual bool IsEnabled => true;

    public abstract void BuildUI();

    public virtual void OnGUI()
    {
    }
}
