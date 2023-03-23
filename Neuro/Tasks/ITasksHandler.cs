﻿using System.Collections;
using Neuro.DependencyInjection;
using UnityEngine;

namespace Neuro.Tasks;

public interface ITasksHandler : IContextAccepter
{
    public IEnumerator EvaluatePath(NormalPlayerTask task);

    public Vector2[] CurrentPath { get; set; }
    
    public int PathIndex { get; set; }
}