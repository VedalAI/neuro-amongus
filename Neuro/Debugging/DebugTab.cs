namespace Neuro.Debugging;

public abstract class DebugTab
{
    public abstract string Name { get; }
    public virtual bool IsEnabled => true;
    public bool LastEnabled = false;

    public abstract void BuildUI();

    public virtual void Awake() { }
    public virtual void OnEnable() { }
    public virtual void OnDisable() { }
    public virtual void Update() { }
}
