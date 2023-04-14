using System;
using AmongUs.GameOptions;
using static Neuro.Recording.Header.HeaderFrame.Types;

namespace Neuro.Recording.Header;

public partial class HeaderFrame
{
    public static HeaderFrame Generate()
    {
        if (!ShipStatus.Instance || !PlayerControl.LocalPlayer) return null;

        HeaderFrame frame = new()
        {
            Map = ShipStatus.Instance.GetTypeForMessage(),
            Role = PlayerControl.LocalPlayer.Data.Role.Role.ForMessage(),
            IsImpostor = PlayerControl.LocalPlayer.Data.Role.IsImpostor
        };

        if (frame.IsImpostor)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.AmOwner && player.Data.Role.IsImpostor)
                {
                    frame.OtherImpostors.Add(player.PlayerId);
                }
            }
        }

        return frame;
    }
}

public static class HeaderFrameExtensions
{
    public static MapType GetTypeForMessage(this ShipStatus shipStatus)
    {
        if (!shipStatus) return MapType.UnsetMapType;
        if (shipStatus.TryCast<AirshipStatus>()) return MapType.Airship;
        switch (shipStatus.Type)
        {
            case ShipStatus.MapType.Ship: return MapType.Skeld;
            case ShipStatus.MapType.Hq: return MapType.MiraHq;
            case ShipStatus.MapType.Pb: return MapType.Polus;
            default:
                Warning($"Unkown ship status type: {shipStatus.GetIl2CppType()} with MapType {shipStatus.Type}");
                return MapType.UnsetMapType;
        }
    }

    public static RoleType ForMessage(this RoleTypes role)
    {
        return Enum.TryParse(role.ToString(), out RoleType result) ? result : RoleType.UnsetRoleType;
    }
}
