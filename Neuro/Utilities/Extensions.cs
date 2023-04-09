using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;

namespace Neuro.Utilities;

public static class Extensions
{
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

    /// <summary>
    /// Filters the elements of an <see cref="IEnumerable"/> based on a specified Il2Cpp type.
    /// </summary>
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

    public static void Write(this BinaryWriter writer, Vector2 vector)
    {
        writer.Write(vector.x);
        writer.Write(vector.y);
    }
}
