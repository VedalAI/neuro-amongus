using Neuro.Recording.DeadBodies;
using Neuro.Recording.LocalPlayer;
using Neuro.Recording.Map;
using Neuro.Recording.OtherPlayers;

namespace Neuro.Recording;

public partial class Frame
{
    public static Frame Now => new()
    {
        DeadBodies = DeadBodiesRecorder.Instance.Frame,
        LocalPlayer = LocalPlayerRecorder.Instance.Frame,
        Map = MapRecorder.Instance.Frame,
        OtherPlayers = OtherPlayersRecorder.Instance.Frame
    };
}
