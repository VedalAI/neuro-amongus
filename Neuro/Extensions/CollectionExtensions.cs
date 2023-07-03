using System;
using System.Collections.Generic;
using System.Linq;

namespace Neuro.Extensions;

public static class CollectionExtensions
{
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