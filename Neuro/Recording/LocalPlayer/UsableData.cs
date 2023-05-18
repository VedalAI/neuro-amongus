using Neuro.Recording.Common;

namespace Neuro.Recording.LocalPlayer;

public partial class UsableData
{
    public static UsableData Create(IUsable usable)
    {
        if (usable == null) return new UsableData() { Type = UsableType.NoneUsableType };

        UsableData data = new()
        {
            Type = usable.GetTypeForMessage()
        };

        if (data.Type is not (UsableType.Ladder or UsableType.FlyingPlatform)) return data;

        //TODO: Get the target movement direction

        return data;
    }
}