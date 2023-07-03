using System;
using System.Collections.Generic;
using HarmonyLib;
using Object = UnityEngine.Object;

namespace Neuro.Caching.Collections;

/// <summary>
/// Represents a <see cref="List{T}"/> in which each value is anchored.
/// If a list element is destroyed, then it is automatically removed.
/// </summary>
public class UnstableList<T> : AnchoredUnstableList<T> where T : Object
{
    public void Add(T item) => base.Add(item, item);

    public void AddRange(IEnumerable<T> items) => items.Do(Add);

    [Obsolete("Use .Add(T) instead.", true)]
    public new void Add(Object anchor, T item) => base.Add(anchor, item);
}