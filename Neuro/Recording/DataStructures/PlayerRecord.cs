using System;

namespace Neuro.Recording.DataStructures;

[Serializable]
public record struct PlayerRecord(float Distance = -1, MyVector2 RelativeDirection = new MyVector2());
