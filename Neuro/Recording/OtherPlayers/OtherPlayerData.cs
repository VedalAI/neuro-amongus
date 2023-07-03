using UnityEngine;

namespace Neuro.Recording.OtherPlayers;

public partial class OtherPlayerData
{
    public static OtherPlayerData Create(PlayerControl player)
    {
        return new OtherPlayerData
        {
            Id = player.PlayerId,
            LastSeenPosition = player.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition(),
            LastSeenTime = Time.fixedTime,
            TimesSawVent = 0,
            RoundTimeVisible = 0,
            GameTimeVisible = 0,
            IsVisible = true
        };
    }

    public void UpdateVisible(PlayerControl owner)
    {
        if (owner.MyPhysics.Animations.IsPlayingEnterVentAnimation() || IsPlayingExitVentAnimation(owner.MyPhysics.Animations))
        {
            TimesSawVent++;
        }

        if (owner.inVent) return;

        LastSeenPosition = owner.GetTruePosition() - PlayerControl.LocalPlayer.GetTruePosition();
        LastSeenTime = Time.fixedTime;
        RoundTimeVisible += Time.fixedDeltaTime;
        GameTimeVisible += Time.fixedDeltaTime;
        IsVisible = true;
    }

    public void ResetAfterMeeting()
    {
        TimesSawVent = 0;
        RoundTimeVisible = 0;
    }

    private static bool IsPlayingExitVentAnimation(PlayerAnimations animations)
    {
        return animations.Animator.GetCurrentAnimation() == animations.group.ExitVentAnim;
    }
}