using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Recording.OtherPlayers;

public partial class OtherPlayerData
{
    public static OtherPlayerData Create(PlayerControl player)
    {
        return new OtherPlayerData
        {
            Id = player.PlayerId,
            LastSeenPosition = player.GetTruePosition(),
            LastSeenTime = Time.fixedTime,
            TimesSawVent = 0,
            RoundTimeVisible = 0,
            GameTimeVisible = 0
        };
    }

    public void UpdateVisible(PlayerControl owner)
    {
        // TODO: Only trigger once per vent
        if (owner.MyPhysics.Animations.IsPlayingEnterVentAnimation() || owner.MyPhysics.Animations.IsPlayingExitVentAnimation())
        {
            TimesSawVent++;
        }

        if (owner.inVent) return;

        LastSeenPosition = owner.GetTruePosition();
        LastSeenTime = Time.fixedTime;
        RoundTimeVisible += Time.fixedDeltaTime;
        GameTimeVisible += Time.fixedDeltaTime;
    }

    public void ResetAfterMeeting()
    {
        TimesSawVent = 0;
        RoundTimeVisible = 0;
    }
}
