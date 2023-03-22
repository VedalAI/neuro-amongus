using BepInEx;
using BepInEx.Unity.IL2CPP;
using Reactor;
using UnityEngine;

namespace Neuro;

[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public class NeuroPlugind : BasePlugin
{
    public LineRenderer arrow;


    public Vector2 directionToNearestTask;
    public Vector2 moveDirection;

    public Vision vision = new();

    public void FixedUpdate()
    {
        // TODO: This method should be split into multiple MonoBehaviours - one for vision, one for recording and one for doing tasks

        if (MeetingHud.Instance) return;

        vision.UpdateVision();
    }
}
