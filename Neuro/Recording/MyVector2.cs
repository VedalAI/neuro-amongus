using UnityEngine;

namespace Neuro.Recording;

public partial class MyVector2
{
    public MyVector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator Vector2(MyVector2 vector) => new(vector.X, vector.Y);
    public static implicit operator Vector3(MyVector2 vector) => new(vector.X, vector.Y);
    public static implicit operator MyVector2(Vector2 vector) => new(vector.x, vector.y);
    public static implicit operator MyVector2(Vector3 vector) => new(vector.x, vector.y);
}
