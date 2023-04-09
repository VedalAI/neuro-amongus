using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Recording.OtherPlayers;

// TODO: ReportFindings was removed
public readonly struct OtherPlayerData : ISerializable
{
    private int Id { get; init; }
    private Vector2 LastSeenPosition { get; init; }
    private float LastSeenTime { get; init; }
    private int SawVent { get; init; }
    private float RoundTimeVisible { get; init; }
    private float GameTimeVisible { get; init; }

    public OtherPlayerData(int id, Vector2 lastSeenPosition, float lastSeenTime, int sawVent, float roundTimeVisible, float gameTimeVisible)
    {
        Id = id;
        LastSeenPosition = lastSeenPosition;
        LastSeenTime = lastSeenTime;
        SawVent = sawVent;
        RoundTimeVisible = roundTimeVisible;
        GameTimeVisible = gameTimeVisible;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Id);
        writer.Write(LastSeenPosition);
        writer.Write(LastSeenTime);
        writer.Write(SawVent);
        writer.Write(RoundTimeVisible);
        writer.Write(GameTimeVisible);
    }

    public static OtherPlayerData Create(PlayerControl player)
    {
        return new OtherPlayerData
        {
            Id = player.PlayerId,
            LastSeenPosition = player.GetTruePosition(),
            LastSeenTime = Time.fixedTime,
            SawVent = 0,
            RoundTimeVisible = 0,
            GameTimeVisible = 0
        };
    }

    public OtherPlayerData UpdateVisible(PlayerControl owner)
    {
        OtherPlayerData result = this;

        if (owner.MyPhysics.Animations.IsPlayingEnterVentAnimation() || owner.MyPhysics.Animations.IsPlayingExitVentAnimation())
        {
            result = result with {SawVent = SawVent + 1};
        }

        if (owner.inVent) return result;

        result = result with
        {
            LastSeenPosition = owner.GetTruePosition(),
            LastSeenTime = Time.fixedTime,
            RoundTimeVisible = RoundTimeVisible + Time.fixedDeltaTime,
            GameTimeVisible = GameTimeVisible + Time.fixedDeltaTime
        };

        return result;
    }

    public OtherPlayerData ResetAfterMeeting()
    {
        return this with
        {
            SawVent = 0,
            RoundTimeVisible = 0
        };
    }
}
