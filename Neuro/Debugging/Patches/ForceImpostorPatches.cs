using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Neuro.Debugging.Tabs;
using Neuro.Extensions;
using Neuro.Extensions.Harmony;

namespace Neuro.Debugging.Patches;

[DebugHarmonyPatch]
public static class ForceImpostorPatches
{
    [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    public static bool SelectRolesPatch()
    {
        if (!ForceImpostorDebugTab.Enabled) return true;
        if (!GameManager.Instance.IsNormal()) return true;

        List<GameData.PlayerInfo> list = GameData.Instance.AllPlayers.ToArray().Where(pcd => !pcd.Disconnected && !pcd.IsDead).ToList();
        List<GameData.PlayerInfo> impostors = new();

        foreach (string name in ForceImpostorDebugTab.CurrentlySelected)
        {
            GameData.PlayerInfo match = list.FirstOrDefault(p => p.PlayerName == name);
            if (match == null || match.Disconnected) continue;
            impostors.Add(match);
        }

        LogicRoleSelectionNormal roleSelectionNormal = GameManager.Instance.LogicRoleSelection.Cast<LogicRoleSelectionNormal>();

        int num1 = 0;
        roleSelectionNormal.AssignRolesFromList(impostors.ToIl2CppList(), impostors.Count,
            Enumerable.Repeat(RoleTypes.Impostor, impostors.Count).ToList().ToIl2CppList(), ref num1);

        int num2 = 0;
        List<GameData.PlayerInfo> crew = list.Where(e => !impostors.Contains(e)).ToList();
        roleSelectionNormal.AssignRolesFromList(crew.ToIl2CppList(), crew.Count,
            Enumerable.Repeat(RoleTypes.Crewmate, crew.Count).ToList().ToIl2CppList(), ref num2);

        return false;
    }
}
