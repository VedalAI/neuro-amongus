using Neuro.Recording.DeadBodies;
using Neuro.Recording.Header;
using Neuro.Recording.LocalPlayer;
using Neuro.Recording.Map;
using Neuro.Recording.OtherPlayers;

namespace Neuro.Recording;

public partial class Frame
{
    public static Frame Now(bool includeHeader = false)
    {
        return new Frame
        {
            DeadBodies = DeadBodiesRecorder.Instance.Frame,
            Header = includeHeader ? HeaderFrame.Generate() : null,
            LocalPlayer = LocalPlayerRecorder.Instance.Frame,
            Map = MapRecorder.Instance.Frame,
            OtherPlayers = OtherPlayersRecorder.Instance.Frame
        };
    }

    public static bool CanGenerate => DeadBodiesRecorder.Instance &&
                                      LocalPlayerRecorder.Instance &&
                                      MapRecorder.Instance &&
                                      OtherPlayersRecorder.Instance &&
                                      ShipStatus.Instance &&
                                      PlayerControl.LocalPlayer;
}
