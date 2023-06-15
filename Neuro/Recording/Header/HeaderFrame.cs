using Neuro.Recording.Common;

namespace Neuro.Recording.Header;

public partial class HeaderFrame
{
    public static HeaderFrame Generate()
    {
        if (!ShipStatus.Instance || !PlayerControl.LocalPlayer) return null;

        HeaderFrame frame = new()
        {
            Version = 3,
            Map = ShipStatus.Instance.GetTypeForMessage(),
            IsFreeplay = TutorialManager.InstanceExists,
            Role = PlayerControl.LocalPlayer.Data.Role.Role.ForMessage(),
            IsImpostor = PlayerControl.LocalPlayer.Data.Role.IsImpostor
        };

        Warning("IS FREEPLAY: " + frame.IsFreeplay);

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
