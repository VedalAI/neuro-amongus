extern alias JetBrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MeansImplicitUse = JetBrains::JetBrains.Annotations.MeansImplicitUseAttribute;

namespace Neuro.Debugging;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[MeansImplicitUse]
public sealed class DebugTabAttribute : Attribute
{
    static DebugTabAttribute()
    {
        Tabs = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .Where(t => t.GetCustomAttribute<DebugTabAttribute>() is not null)
            .Where(t => t.IsAssignableTo(typeof(DebugTab)))
            .Select(Activator.CreateInstance)
            .OfType<DebugTab>()
            .OrderBy(t => t.Name)
            .ToList();
    }

    public static List<DebugTab> Tabs { get; }
}
