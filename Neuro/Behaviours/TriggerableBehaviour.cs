using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Behaviours;

[RegisterInIl2Cpp]
public abstract class TriggerableBehaviour : MonoBehaviour
{
    protected TriggerableBehaviour(nint ptr) : base(ptr)
    {
    }

    private bool _hasBeenTriggered;

    [HideFromIl2Cpp] public abstract bool IsTriggered { get; }

    protected virtual void Awake()
    {
        EventManager.RegisterHandler(this);
    }

    protected virtual void FixedUpdate()
    {
        if (!_hasBeenTriggered && IsTriggered)
        {
            _hasBeenTriggered = true;
            OnTrigger();
        }

        if (_hasBeenTriggered && !IsTriggered)
        {
            _hasBeenTriggered = false;
            OnEnd();
        }
    }

    [HideFromIl2Cpp]
    protected virtual void OnTrigger()
    {
    }

    [HideFromIl2Cpp]
    protected virtual void OnEnd()
    {
    }

    [EventHandler(EventTypes.ExileCutsceneEnded)]
    [HideFromIl2Cpp]
    protected virtual void ResetAfterMeeting()
    {
    }
}
