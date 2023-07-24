using System.Collections.Generic;
using System.Linq;
using Il2CppInterop.Runtime.Attributes;
using Neuro.Events;
using Neuro.Recording.OtherPlayers;
using Neuro.Recording.Tasks;
using Reactor.Utilities.Attributes;

namespace Neuro.Behaviours;

[RegisterInIl2Cpp, FullShipStatusComponent]
public sealed class SuggestMeetingButtonWhenSawPlayerVent : TriggerableBehaviour
{
    public SuggestMeetingButtonWhenSawPlayerVent(nint ptr) : base(ptr)
    {
    }

    [HideFromIl2Cpp] public HashSet<uint> VentingPlayers { get; } = new();

    [HideFromIl2Cpp] public override bool IsTriggered => VentingPlayers.Any();

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (MeetingHud.Instance) return;
        if (!OtherPlayersRecorder.Instance) return;

        foreach (OtherPlayerData data in OtherPlayersRecorder.Instance.Frame.LastSeenPlayers)
        {
            if (VentingPlayers.Contains(data.Id)) continue;

            GameData.PlayerInfo player = GameData.Instance.GetPlayerById((byte) data.Id);
            if (!player.Role.IsImpostor) continue; // ignoring engineers - perfectly balanced, lmao (this is peak AI development)

            if (data.TimesSawVent > 0)
            {
                VentingPlayers.Add(data.Id);
            }
        }
    }

    [HideFromIl2Cpp]
    protected override void OnTrigger()
    {
        MovementSuggestion.Instance.SuggestMeetingButton();
    }

    [HideFromIl2Cpp]
    protected override void OnEnd()
    {
        MovementSuggestion.Instance.ClearSuggestion();
    }

    [HideFromIl2Cpp]
    protected override void ResetAfterMeeting()
    {
        VentingPlayers.Clear();
    }
}
