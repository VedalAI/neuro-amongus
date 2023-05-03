﻿using Neuro.Recording.DeadBodies;
using Neuro.Recording.Header;
using Neuro.Recording.LocalPlayer;
using Neuro.Recording.Map;
using Neuro.Recording.OtherPlayers;
using Neuro.Recording.Tasks;

namespace Neuro.Recording;

public partial class Frame
{
    public static Frame Now(bool includeHeader = false)
    {
        try
        {
            return new Frame
            {
                DeadBodies = DeadBodiesRecorder.Instance.Frame,
                Header = includeHeader ? HeaderFrame.Generate() : null,
                LocalPlayer = LocalPlayerRecorder.Instance.Frame,
                Map = MapRecorder.Instance.Frame,
                OtherPlayers = OtherPlayersRecorder.Instance.Frame,
                Tasks = TasksRecorder.Instance.Frame,
            };
        }
        finally
        {
            LocalPlayerRecorder.Instance.Cleanup();
        }
    }

    public static bool CanGenerate => ShipStatus.Instance && PlayerControl.LocalPlayer &&
                                      DeadBodiesRecorder.Instance &&
                                      LocalPlayerRecorder.Instance &&
                                      MapRecorder.Instance &&
                                      OtherPlayersRecorder.Instance &&
                                      TasksRecorder.Instance;
}
