namespace Neuro.Communication.AmongUsAI.DataStructures;

public readonly struct VentData
{
    public PositionData Position { get; init; }
    public PositionData[] ConnectingVents { get; init; }
}
