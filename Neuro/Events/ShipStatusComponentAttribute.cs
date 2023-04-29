using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using UnityEngine;

namespace Neuro.Events;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ShipStatusComponentAttribute : Attribute
{
    static ShipStatusComponentAttribute()
    {
        ShipStatusComponents = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .Where(t => t.GetCustomAttribute<ShipStatusComponentAttribute>() is not null)
            .Where(t => t.IsAssignableTo(typeof(Component)))
            .ToList();
    }

    private static List<Type> ShipStatusComponents { get; }

    [EventHandler(EventTypes.GameStarted)]
    private static void OnGameStarted()
    {
        foreach (Type component in ShipStatusComponents)
        {
            ShipStatus.Instance.gameObject.AddComponent(Il2CppType.From(component));
        }
    }
}
