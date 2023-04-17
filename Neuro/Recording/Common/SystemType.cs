using System;

namespace Neuro.Recording.Common;

public static class SystemTypeExtensions
{
    public static SystemType ForMessage(this SystemTypes system)
    {
        return Enum.TryParse(system.ToString(), out SystemType result) ? result : SystemType.NoneSystemType;
    }
}
