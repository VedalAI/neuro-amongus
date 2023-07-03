using System;
using System.Collections;
using System.Linq;
using BepInEx.Unity.IL2CPP.Utils;
using Neuro.Cursor;
using Neuro.Events;
using Reactor.Utilities.Attributes;
using UnityEngine;

namespace Neuro.Impostor;

[RegisterInIl2Cpp]
public sealed class MeetingHandler : MonoBehaviour
{
    public MeetingHandler(IntPtr ptr) : base(ptr)
    {
    }

    public static MeetingHandler Instance { get; private set; }

    private bool _voted = false;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void HighlightPlayer(byte playerId)
    {
        if (MeetingHud.Instance.CurrentState != MeetingHud.VoteStates.NotVoted) return;

        this.StartCoroutine(CoHighlight(GetVoteAreaForPlayer(playerId)));
    }

    public void HighlightSkip()
    {
        if (MeetingHud.Instance.CurrentState != MeetingHud.VoteStates.NotVoted) return;

        this.StartCoroutine(CoHighlight(MeetingHud.Instance.SkipVoteButton));
    }

    public void VoteForPlayer(byte playerId)
    {
        if (MeetingHud.Instance.CurrentState != MeetingHud.VoteStates.NotVoted) return;

        if (_voted) return;
        _voted = true;

        this.StartCoroutine(CoVoteFor(GetVoteAreaForPlayer(playerId)));
    }

    public void VoteForSkip()
    {
        if (MeetingHud.Instance.CurrentState != MeetingHud.VoteStates.NotVoted) return;

        if (_voted) return;
        _voted = true;

        this.StartCoroutine(CoVoteFor(MeetingHud.Instance.SkipVoteButton));
    }

    private static IEnumerator CoHighlight(PlayerVoteArea votedPlayer)
    {
        InGameCursor.Instance.HideWhen(() => !MeetingHud.Instance);
        yield return InGameCursor.Instance.CoMoveTo(votedPlayer);
        yield return InGameCursor.Instance.CoPressLMB();
    }

    private static IEnumerator CoVoteFor(PlayerVoteArea votedPlayer)
    {
        yield return CoHighlight(votedPlayer);
        yield return new WaitForSeconds(0.35f);
        yield return InGameCursor.Instance.CoMoveTo(votedPlayer.ConfirmButton);
        yield return InGameCursor.Instance.CoPressLMB();
        InGameCursor.Instance.Hide();
    }

    private static PlayerVoteArea GetVoteAreaForPlayer(byte playerId) => MeetingHud.Instance.playerStates.Single(s => s.TargetPlayerId == playerId);

    [EventHandler(EventTypes.MeetingStarted)]
    private static void CreateMeetingHandler(MeetingHud meetingHud)
    {
        meetingHud.gameObject.AddComponent<MeetingHandler>();
    }
}
