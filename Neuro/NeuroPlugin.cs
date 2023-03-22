﻿using System.Reflection;
using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Neuro.DependencyInjection;
using Neuro.Pathfinding;
using Neuro.Visibility;
using Neuro.Vision;
using Reactor;

namespace Neuro;

[BepInAutoPlugin]
[BepInProcess("Among Us.exe")]
[BepInDependency(ReactorPlugin.Id)]
public partial class NeuroPlugin : BasePlugin, IContextProvider
{
    public IContextProvider MainContext => this;

    public IPathfindingHandler PathfindingHandler { get; private set; }
    public IVisibilityHandler VisibilityHandler { get; }
    public IVisionHandler VisionHandler { get; }

    public override void Load()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), Id);

        PathfindingHandler = new PathfindingHandler();
    }
}
