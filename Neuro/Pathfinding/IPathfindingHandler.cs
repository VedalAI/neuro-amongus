﻿using Neuro.DependencyInjection;
using UnityEngine;

namespace Neuro.Pathfinding;

public interface IPathfindingHandler : IContextAcceptor
{
    public void Initialize();

    public Vector2[] FindPath(Vector2 start, Vector2 end);
}
