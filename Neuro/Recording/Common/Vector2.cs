using U = UnityEngine;

namespace Neuro.Recording.Common;

public partial class Vector2
{
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public static implicit operator U.Vector2(Vector2 vector) => new(vector.X, vector.Y);
    public static implicit operator U.Vector3(Vector2 vector) => new(vector.X, vector.Y);
    public static implicit operator Vector2(U.Vector2 vector) => new(vector.x, vector.y);
    public static implicit operator Vector2(U.Vector3 vector) => new(vector.x, vector.y);
}
