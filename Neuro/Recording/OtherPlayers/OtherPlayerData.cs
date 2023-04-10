using System.IO;
using Neuro.Communication.AmongUsAI;
using Neuro.Utilities;
using UnityEngine;

namespace Neuro.Recording.OtherPlayers;

public readonly struct OtherPlayerData : ISerializable
{
    public byte Id { get; init; }
    public Vector2 LastSeenPosition { get; init; }
    public float LastSeenTime { get; init; }
    public byte TimesSawVent { get; init; }
    public float RoundTimeVisible { get; init; }
    public float GameTimeVisible { get; init; }

    public OtherPlayerData(byte id, Vector2 lastSeenPosition, float lastSeenTime, byte timesSawVent, float roundTimeVisible, float gameTimeVisible)
    {
        Id = id;
        LastSeenPosition = lastSeenPosition;
        LastSeenTime = lastSeenTime;
        TimesSawVent = timesSawVent;
        RoundTimeVisible = roundTimeVisible;
        GameTimeVisible = gameTimeVisible;
    }

    public void Serialize(BinaryWriter writer)
    {
        writer.Write(Id);
        writer.Write(LastSeenPosition);
        writer.Write(LastSeenTime);
        writer.Write(TimesSawVent);
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
            TimesSawVent = 0,
            RoundTimeVisible = 0,
            GameTimeVisible = 0
        };
    }

    public OtherPlayerData UpdateVisible(PlayerControl owner)
    {
        OtherPlayerData result = this;

        if (owner.MyPhysics.Animations.IsPlayingEnterVentAnimation() || owner.MyPhysics.Animations.IsPlayingExitVentAnimation())
        {
            result = result with {TimesSawVent = (byte) (TimesSawVent + 1)};
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
            TimesSawVent = 0,
            RoundTimeVisible = 0
        };
    }
}
