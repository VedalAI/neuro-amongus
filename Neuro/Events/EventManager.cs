using System.Collections.Generic;
using System.Reflection;
using Neuro.Utilities.Collections;
using UnityEngine;

namespace Neuro.Events;

public static class EventManager
{
    public static UnstableList<Object> HandlerInstances { get; } = new();

    public static void RegisterHandler<T>(T target) where T : Object
    {
        HandlerInstances.Add(target);
    }

    public static void InvokeEvent(EventTypes eventType)
    {
        if (EventHandlerAttribute.StaticEvents.TryGetValue(eventType, out List<MethodInfo> staticEvents))
        {
            foreach (MethodInfo eventHandler in staticEvents)
            {
                eventHandler.Invoke(null, null);
            }
        }

        if (EventHandlerAttribute.InstanceEvents.TryGetValue(eventType, out List<MethodInfo> instanceEvents))
        {
            foreach (MethodInfo eventHandler in instanceEvents)
            {
                foreach (Object obj in HandlerInstances)
                {
                    if (obj.GetType() == eventHandler.DeclaringType)
                    {
                        eventHandler.Invoke(obj, null);
                    }
                }
            }
        }
    }
}
