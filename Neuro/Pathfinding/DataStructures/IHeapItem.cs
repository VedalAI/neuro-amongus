using System;

namespace Neuro.Pathfinding.DataStructures;

public interface IHeapItem<in T> : IComparable<T>
{
    int HeapIndex { get; set; }
}
