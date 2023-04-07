using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppSystem.Net.NetworkInformation;

namespace Neuro.Events;

[AttributeUsage(AttributeTargets.Method)]
public sealed class EventHandlerAttribute : Attribute
{
    static EventHandlerAttribute()
    {
        StaticEvents = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => m.IsStatic)
            .Where(m => m.GetCustomAttribute<EventHandlerAttribute>() is not null)
            .SelectMany(m => m.GetCustomAttribute<EventHandlerAttribute>()!.Events.Select(e => (eventType: e, method: m)))
            .GroupBy(i => i.eventType)
            .ToDictionary(i => i.Key, i => i.Select(x => x.method).ToList());

        InstanceEvents = Assembly.GetExecutingAssembly().GetTypes()
            .SelectMany(t => t.GetMethods())
            .Where(m => !m.IsStatic && m.HasMethodBody())
            .Where(m => m.GetCustomAttribute<EventHandlerAttribute>() is not null)
            .SelectMany(m => m.GetCustomAttribute<EventHandlerAttribute>()!.Events.Select(e => (eventType: e, method: m)))
            .GroupBy(i => i.eventType)
            .ToDictionary(i => i.Key, i => i.Select(x => x.method).ToList());
    }

    public static Dictionary<EventTypes, List<MethodInfo>> StaticEvents { get; }
    public static Dictionary<EventTypes, List<MethodInfo>> InstanceEvents { get; }

    public EventHandlerAttribute(EventTypes firstEvent, params EventTypes[] otherEvents)
    {
        Events = otherEvents.AddToArray(firstEvent);
    }

    public readonly EventTypes[] Events;
}
