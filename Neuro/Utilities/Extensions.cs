using System;

namespace Neuro.Utilities;

public static class Extensions
{
    /// <summary>
    /// This returns the element at the specified index from the il2cpp list.
    /// JetBrains Rider will complain about an ambiguous indexer if used normally (list[i]), so we inline it's indexer.
    /// </summary>
    public static T At<T>(this Il2CppSystem.Collections.Generic.List<T> list, int index)
    {
        if ((uint)index >= (uint)list._size)
        {
            throw new ArgumentOutOfRangeException(nameof(index), "Index was out of range. Must be non-negative and less than the size of the collection.");
        }

        return list._items[index];
    }
}
