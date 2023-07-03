using System;
using System.Diagnostics;
using HarmonyLib;

namespace Neuro.Extensions.Harmony;

[AttributeUsage(AttributeTargets.Method)]
[Conditional("FULL")]
public sealed class FullHarmonyPostfixAttribute : HarmonyPostfix
{
}