using Neuro.DependencyInjection;
using UnityEngine;

namespace Neuro.Vision;

public interface IVisionHandler : IContextAccepter
{
    public void StartTrackingPlayer(PlayerControl player);

    public void AddDeadBody(DeadBody body);

    public void ReportFindings();

    public void ResetAfterMeeting();

    public Vector2 DirectionToNearestBody { get; }
}
