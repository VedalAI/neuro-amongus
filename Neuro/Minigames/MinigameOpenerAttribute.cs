extern alias JetBrains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime;
using MeansImplicitUse = JetBrains::JetBrains.Annotations.MeansImplicitUseAttribute;

namespace Neuro.Minigames;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
[MeansImplicitUse]
public class MinigameOpenerAttribute : Attribute
{
    static MinigameOpenerAttribute()
    {
        MinigameOpeners = AccessTools.GetTypesFromAssembly(Assembly.GetExecutingAssembly())
            .Where(type => type.IsAssignableTo(typeof(IMinigameOpener)))
            .SelectMany(opener => opener.GetCustomAttributes<MinigameOpenerAttribute>()
                .Select(attribute => (opener, minigame: attribute.Type)))
            .Where(item => item.minigame is not null)
            .GroupBy(item => item.minigame)
            .ToDictionary(group => Il2CppType.From(group.Key).FullName,
                group => group.Select(item => (IMinigameOpener) Activator.CreateInstance(item.opener)).ToList());
    }

    public static Dictionary<string, List<IMinigameOpener>> MinigameOpeners { get; }

    public MinigameOpenerAttribute(Type type)
    {
        Type = type;
    }

    public readonly Type Type;
}
