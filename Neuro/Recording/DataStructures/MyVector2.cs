using System;
using UnityEngine;

namespace Neuro.Recording.DataStructures;

// TODO: Can this be replaced with System.Numerics.Vector2?
[Serializable]
public record struct MyVector2(float x, float y)
{
    public static implicit operator MyVector2(Vector2 v) => new(v.x, v.y);
}
