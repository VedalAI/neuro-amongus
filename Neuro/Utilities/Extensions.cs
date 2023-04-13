using System;
using System.Collections;
using System.Collections.Generic;
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
    public static T At<T>(this Il2CppSystem.Collections.Generic.List<T> list, int index)
    {
        if ((uint)index >= (uint)list._size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        return list._items[index];
    }

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
        return (T)_castMethod.MakeGenericMethod(obj.GetIl2CppType().ToSystemType()).Invoke(obj, null);
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
}
