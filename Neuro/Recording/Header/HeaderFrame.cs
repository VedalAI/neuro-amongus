using Neuro.Recording.Common;

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
