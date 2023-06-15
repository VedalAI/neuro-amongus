using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Neuro.Pathfinding.DataStructures;

public sealed class PlatformNode : Node
{
    private readonly PlatformConsole _platform;

    public PlatformNode(Vector2 worldPosition, Vector2Int gridPosition, bool isAccessible, PlatformConsole platform)
        : base(worldPosition, gridPosition, isAccessible)
    {
        _platform = platform;
    }

    public override bool IsTransportActive
    {
        get
        {
            Vector3 currentPosition = _platform.Platform.transform.position;
            if (currentPosition == _lastPosition) return _lastResult;

            return _lastResult = Vector2.Distance(_lastPosition = currentPosition, _platform.transform.position) < 2f;
        }
    }

    private Vector3 _lastPosition;
    private bool _lastResult;
}
