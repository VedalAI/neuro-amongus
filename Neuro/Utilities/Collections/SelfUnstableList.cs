using System;
using System.Collections.Generic;
using HarmonyLib;
using Object = UnityEngine.Object;

namespace Neuro.Utilities.Collections;

public class SelfUnstableList<T> : UnstableList<T> where T : Object
{
    public void Add(T item) => base.Add(item, item);

    public void AddRange(IEnumerable<T> items) => items.Do(Add);

    [Obsolete("Use .Add(T) instead.", true)]
    public new void Add(Object anchor, T item) => base.Add(anchor, item);
}
