using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes;
using Reactor.Utilities.Extensions;

namespace Neuro.Utilities;

public static class Extensions
{
    private static readonly MethodInfo _castMethod = AccessTools.Method(typeof(Il2CppObjectBase), nameof(Il2CppObjectBase.Cast));

    /// <summary>
    /// This returns the element at the specified index from the il2cpp list.
    /// JetBrains Rider will complain about an ambiguous indexer if used normally (list[i]).
    /// </summary>
    public static T At<T>(this Il2CppSystem.Collections.Generic.List<T> list, int index) => list._items[index];

    /// <summary>
    /// This returns the element at the specified index from the il2cpp list.
    /// JetBrains Rider will complain about an ambiguous indexer if used normally (list[i]).
    /// </summary>
    public static T At<T>(this Il2CppSystem.Collections.Generic.List<T> list, Index index) => list._items[index];

    public static IEnumerable<T> OfIl2CppType<T>(this IEnumerable collection) where T : Il2CppObjectBase
    {
        foreach (object obj in collection)
        {
            if (obj is not Il2CppObjectBase il2cppObject) continue;

            T newObj = il2cppObject.TryCast<T>();
            if (newObj != null) yield return newObj;
        }
    }

    public static bool IsPlayingExitVentAnimation(this PlayerAnimations animations)
    {
        return animations.Animator.GetCurrentAnimation() == animations.group.ExitVentAnim;
    }

    public static T Il2CppCastToTopLevel<T>(this T obj) where T : Il2CppSystem.Object
    {
        if (obj == null) return null;
        return (T) _castMethod.MakeGenericMethod(obj.GetIl2CppType().ToSystemType()).Invoke(obj, null);
    }

    public static Il2CppSystem.Object Il2CppCastToTopLevel(this Il2CppObjectBase obj)
        => Il2CppCastToTopLevel(obj.Cast<Il2CppSystem.Object>());

    public static void FillWithDefault<T>(this ICollection<T> repeatedField, int capacity)
    {
        for (int i = repeatedField.Count; i < capacity; i++)
        {
            repeatedField.Add(default);
        }
    }

    public static IEnumerable<T> ReverseSection<T>(this IList<T> list, Range range)
    {
        int startIndex = range.Start.IsFromEnd ? list.Count - range.Start.Value : range.Start.Value;
        int endIndex = range.End.IsFromEnd ? list.Count - range.End.Value : range.End.Value;

        if (startIndex < 0 || endIndex > list.Count || startIndex >= endIndex)
        {
            throw new ArgumentException("Invalid range.");
        }

        return list.Take(startIndex)
            .Concat(list.Skip(startIndex).Take(endIndex - startIndex).Reverse())
            .Concat(list.Skip(endIndex));
    }

    /// <summary>
    /// Convert a System List to an Il2Cpp List
    /// </summary>
    public static Il2CppSystem.Collections.Generic.List<T> ToIl2CppList<T>(this List<T> systemList)
    {
        Il2CppSystem.Collections.Generic.List<T> iList = new();
        foreach (T item in systemList) iList.Add(item);
        return iList;
    }
}
