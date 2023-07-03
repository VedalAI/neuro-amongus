using System;
using System.Diagnostics;
using HarmonyLib;

namespace Neuro.Extensions.Harmony;

[AttributeUsage(AttributeTargets.Method)]
[Conditional("DEBUG")]
public sealed class DebugHarmonyPrefixAttribute : HarmonyPrefix
{
}
