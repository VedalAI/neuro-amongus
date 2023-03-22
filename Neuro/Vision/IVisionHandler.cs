using Neuro.DependencyInjection;
using UnityEngine;

namespace Neuro.Vision;

public interface IVisionHandler : IContextAcceptor
{
    public void ReportFindings();

    public void ResetAfterMeeting();

    public Vector2 DirectionToNearestBody { get; }
}
