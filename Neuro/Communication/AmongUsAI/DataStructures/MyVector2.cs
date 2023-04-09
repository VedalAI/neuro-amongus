using System;
using UnityEngine;

namespace Neuro.Communication.AmongUsAI.DataStructures;

public readonly record struct MyVector2(float x, float y)
{
    public float magnitude => (float) Math.Sqrt(x * x + y * y);

    public static float Distance(MyVector2 a, MyVector2 b) => (a - b).magnitude;

    public static MyVector2 operator -(MyVector2 a, MyVector2 b) => new(a.x - b.x, a.y - b.y);
    public static MyVector2 operator *(MyVector2 v, float f) => new(v.x * f, v.y * f);
    public static MyVector2 operator *(float f, MyVector2 v) => new(v.x * f, v.y * f);

    public static implicit operator MyVector2(Vector2 v) => new(v.x, v.y);
    public static implicit operator Vector2(MyVector2 v) => new(v.x, v.y);
}
