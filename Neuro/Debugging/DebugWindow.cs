using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neuro.Debugging;

public abstract class DebugWindow
{
    static DebugWindow()
    {
        Tabs = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<DebugWindowAttribute>() is { })
            .Where(t => t.IsAssignableTo(typeof(DebugWindow)))
            .Select(Activator.CreateInstance)
            .OfType<DebugWindow>()
            .ToList();
    }

    public static List<DebugWindow> Tabs { get; }

    public abstract string Name { get; }
    public virtual bool ShouldShow() => true;
    public abstract void BuildWindow();
}
