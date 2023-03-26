namespace Neuro.Utilities;

public static class Extensions
{
    /// <summary>
    /// This returns the element at the specified index from the il2cpp list.
    /// JetBrains Rider will complain about an ambiguous indexer if used normally (list[i]), so we call .ToArray on the list first.
    /// This does produce additional allocations, but the impact is negligible.
    /// </summary>
    public static T At<T>(this Il2CppSystem.Collections.Generic.List<T> list, int index) => list.ToArray()[index]; // TODO: Use reflection to get the indexer
}
