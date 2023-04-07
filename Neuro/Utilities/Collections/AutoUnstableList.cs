using UnityEngine;

namespace Neuro.Utilities.Collections;

public class AutoUnstableList<T> : UnstableList<T> where T : Object
{
    public void Add(T item) => Add(item, item);
}
