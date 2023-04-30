using UnityEngine;

namespace Neuro.Pathfinding.DataStructures;

public sealed class PlatformNode : Node
{
    private readonly PlatformConsole _platform;

    public PlatformNode(Vector2 worldPosition, Vector2Int gridPosition, bool isAccessible, PlatformConsole platform)
        : base(worldPosition, gridPosition, isAccessible)
    {
        _platform = platform;
    }

    public override bool IsTransportActive => Vector2.Distance(_platform.Platform.transform.position, _platform.transform.position) < 2f;
}
