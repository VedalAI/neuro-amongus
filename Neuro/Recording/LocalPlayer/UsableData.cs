using Neuro.Recording.Common;
using Vector2 = UnityEngine.Vector2;

namespace Neuro.Recording.LocalPlayer;

public partial class UsableData
{
    public static UsableData Create(IUsable usable)
    {
        if (usable == null) return new UsableData() {Type = UsableType.NoneUsableType};

        UsableData data = new()
        {
            Type = usable.GetTypeForMessage()
        };

        switch (data.Type)
        {
            case UsableType.Ladder:
                Ladder ladder = usable.Cast<Ladder>();
                data.Direction = ladder.Destination.transform.position.y > ladder.transform.position.y ? Vector2.up : Vector2.down;
                break;

            case UsableType.FlyingPlatform:
                data.Direction = usable.Cast<PlatformConsole>().Platform.IsLeft ? Vector2.right : Vector2.left;
                break;
        }

        return data;
    }
}
