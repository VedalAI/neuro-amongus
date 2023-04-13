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
            Map = ShipStatus.Instance.GetHeaderMapType(),
            Role = PlayerControl.LocalPlayer.Data.Role.Role.ToHeaderRoleType(),
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
    public static MapType GetHeaderMapType(this ShipStatus shipStatus)
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

    public static RoleType ToHeaderRoleType(this RoleTypes role)
    {
        switch (role)
        {
            case RoleTypes.Crewmate: return RoleType.Crewmate;
            case RoleTypes.Impostor: return RoleType.Impostor;
            case RoleTypes.Scientist: return RoleType.Scientist;
            case RoleTypes.Engineer: return RoleType.Engineer;
            case RoleTypes.GuardianAngel: return RoleType.GuardianAngel;
            case RoleTypes.Shapeshifter: return RoleType.Shapeshifter;
            case RoleTypes.CrewmateGhost: return RoleType.UnsetRoleType;
            case RoleTypes.ImpostorGhost: return RoleType.UnsetRoleType;
            default:
                Warning($"Unkown role type: {role}");
                return RoleType.UnsetRoleType;
        }
    }
}
