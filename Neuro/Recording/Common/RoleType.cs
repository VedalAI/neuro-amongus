using System;
using AmongUs.GameOptions;

namespace Neuro.Recording.Common;

public static class RoleTypeExtensions
{
    public static RoleType ForMessage(this RoleTypes role)
    {
        return Enum.TryParse(role.ToString(), out RoleType result) ? result : RoleType.NoneRoleType;
    }
}
