using Neuro.Extensions;

namespace Neuro.Recording.Common;

public static class UsableTypeExtensions
{
    public static UsableType GetTypeForMessage(this IUsable usable)
    {
        return usable.Il2CppCastToTopLevel() switch
        {
            Console _ => UsableType.Console,
            DeconControl _ => UsableType.DecontaminationDoor,
            DoorConsole _ => UsableType.SabotageDoor,
            Ladder _ => UsableType.Ladder,
            OpenDoorConsole _ => UsableType.InsignificantDoor,
            PlatformConsole _ => UsableType.FlyingPlatform,
            SystemConsole _ => UsableType.SystemConsole,
            Vent _ => UsableType.Vent,
            _ => UsableType.NoneUsableType
        };
    }
}