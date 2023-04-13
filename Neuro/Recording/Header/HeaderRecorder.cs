namespace Neuro.Recording.Header;

public static class HeaderRecorder
{
    public static HeaderFrame GenerateHeaderFrame()
    {
        if (!HudManager.Instance.IsIntroDisplayed) return null;

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
