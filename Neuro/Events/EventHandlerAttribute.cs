extern alias JetBrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MeansImplicitUse = JetBrains::JetBrains.Annotations.MeansImplicitUseAttribute;

namespace Neuro.Events;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
[MeansImplicitUse]
public sealed class EventHandlerAttribute : Attribute
{
    static EventHandlerAttribute()
    {
        StaticEvents = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .SelectMany(AccessTools.GetDeclaredMethods)
            .Where(m => m.IsStatic)
            .Where(m => m.GetCustomAttribute<EventHandlerAttribute>() is not null)
            .GroupBy(m => m.GetCustomAttribute<EventHandlerAttribute>()!.EventType)
            .ToDictionary(i => i.Key, i => i.ToList());

        InstanceEvents = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .SelectMany(AccessTools.GetDeclaredMethods)
            .Where(m => !m.IsStatic && m.HasMethodBody())
            .Where(m => m.GetCustomAttribute<EventHandlerAttribute>() is not null)
            .GroupBy(m => m.GetCustomAttribute<EventHandlerAttribute>()!.EventType)
            .ToDictionary(i => i.Key, i => i.ToList());
    }

    public static Dictionary<EventTypes, List<MethodInfo>> StaticEvents { get; }
    public static Dictionary<EventTypes, List<MethodInfo>> InstanceEvents { get; }

    public EventHandlerAttribute(EventTypes eventType)
    {
        EventType = eventType;
    }

    public readonly EventTypes EventType;
}
