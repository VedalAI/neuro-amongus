using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Neuro.Recording.DataStructures;

[Serializable]
public record struct MyVector2(float x, float y)
{
    // TODO: Why is this necessary?
    public static implicit operator MyVector2(Vector2 v) => new(v.x, v.y);
}
