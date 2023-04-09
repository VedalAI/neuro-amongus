using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Recording.OtherPlayers;

// TODO: ReportFindings was removed
public readonly struct OtherPlayerData : ISerializable
{
    public Vector2 LastSeenPosition { get; init; }
    public float LastSeenTime { get; init; }
    public bool SawVent { get; init; }
    public float RoundTimeVisible { get; init; }
    public float GameTimeVisible { get; init; }

    public OtherPlayerData(Vector2 lastSeenPosition, float lastSeenTime, bool sawVent, float roundTimeVisible, float gameTimeVisible)
    {
        LastSeenPosition = lastSeenPosition;
        LastSeenTime = lastSeenTime;
        SawVent = sawVent;
        RoundTimeVisible = roundTimeVisible;
        GameTimeVisible = gameTimeVisible;
    }

    public void Serialize(BinaryWriter writer)
    {
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
            LastSeenPosition = player.GetTruePosition(),
            LastSeenTime = Time.fixedTime,
            SawVent = false,
            RoundTimeVisible = 0,
            GameTimeVisible = 0
        };
    }

    public OtherPlayerData UpdateVisible(PlayerControl owner)
    {
        OtherPlayerData result = this;

        if (owner.MyPhysics.Animations.IsPlayingEnterVentAnimation() || owner.MyPhysics.Animations.IsPlayingExitVentAnimation())
        {
            result = result with {SawVent = true};
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
            SawVent = false,
            RoundTimeVisible = 0
        };
    }
}
