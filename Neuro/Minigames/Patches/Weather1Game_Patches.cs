using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace Neuro.Minigames.Patches;

[HarmonyPatch(typeof(Weather1Game), nameof(Weather1Game.SolveMaze))]
public class Weather1Game_SolveMaze
{
    public static HashSet<Vector3Int> MazeSolution { get; private set; }

    [HarmonyPostfix]
    public static void Postfix(HashSet<Vector3Int> solution)
    {
        MazeSolution = solution;
    }
}