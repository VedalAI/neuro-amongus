extern alias JetBrains;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using UnityEngine;
using MeansImplicitUse = JetBrains::JetBrains.Annotations.MeansImplicitUseAttribute;

namespace Neuro.Events;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[MeansImplicitUse]
public class ShipStatusComponentAttribute : Attribute
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

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
[Conditional("FULL")]
public sealed class FullShipStatusComponentAttribute : ShipStatusComponentAttribute
{
}
