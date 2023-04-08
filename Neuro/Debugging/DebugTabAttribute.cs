using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Neuro.Debugging;

[AttributeUsage(AttributeTargets.Class)]
public sealed class DebugTabAttribute : Attribute
{
    static DebugTabAttribute()
    {
        Tabs = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetCustomAttribute<DebugTabAttribute>() is { })
            .Where(t => t.IsAssignableTo(typeof(DebugTab)))
            .Select(Activator.CreateInstance)
            .OfType<DebugTab>()
            .ToList();
    }

    public static List<DebugTab> Tabs { get; }
}
