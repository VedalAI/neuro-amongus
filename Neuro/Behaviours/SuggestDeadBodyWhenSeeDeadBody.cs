using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Caching;
using Neuro.Caching.Collections;
using Neuro.Events;
using Neuro.Recording.DeadBodies;
using Neuro.Recording.Tasks;
using Reactor.Utilities.Attributes;

namespace Neuro.Behaviours;

[RegisterInIl2Cpp, FullShipStatusComponent]
public sealed class SuggestDeadBodyWhenSeeDeadBody : TriggerableBehaviour
{
    public SuggestDeadBodyWhenSeeDeadBody(nint ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp] public UnstableList<DeadBody> DeadBodies { get; } = new();

    [HideFromIl2Cpp] public override bool IsTriggered => DeadBodies.Any();

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor) return;
        if (MeetingHud.Instance) return;
        if (!DeadBodiesRecorder.Instance) return;

        foreach (DeadBodyData data in DeadBodiesRecorder.Instance.Frame.DeadBodies)
        {
            if (DeadBodies.Any(d => d.ParentId == data.ParentId)) continue;

            DeadBody body = ComponentCache<DeadBody>.Cached.FirstOrDefault(d => d.ParentId == data.ParentId);
            if (!body) continue; // this should never happen

            DeadBodies.Add(body);
        }
    }

    [HideFromIl2Cpp]
    protected override void OnTrigger()
    {
        MovementSuggestion.Instance.SuggestTarget<SuggestDeadBodyWhenSeeDeadBody>(DeadBodies[0]);
    }

    [HideFromIl2Cpp]
    protected override void OnEnd()
    {
        MovementSuggestion.Instance.ClearSuggestion<SuggestDeadBodyWhenSeeDeadBody>();
    }
}
