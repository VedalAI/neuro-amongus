using System;

namespace Neuro.Recording.DataStructures;

[Serializable]
public record struct PlayerRecord(MyVector2 relativeDirection, float distance);
