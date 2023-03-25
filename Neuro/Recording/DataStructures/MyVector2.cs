using System;
using UnityEngine;

namespace Neuro.Recording.DataStructures;

// TODO: Use UnityEngine.Vector2 instead and either: 1. Create a custom JsonConverter or 2. Use a custom binary converter
[Serializable]
public record struct MyVector2(float x, float y)
{
    public static implicit operator MyVector2(Vector2 v) => new(v.x, v.y);
}
