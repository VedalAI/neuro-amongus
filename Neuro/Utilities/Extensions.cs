using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.InteropTypes;

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
            if (obj is not Il2CppObjectBase il2cppObject)
            {
                throw new ArgumentException("OfIl2CppType may only be used for collections of Il2Cpp objects.", nameof(collection));
            }

            T newObj = il2cppObject.TryCast<T>();
            if (newObj != null) yield return newObj;
        }
    }
}
