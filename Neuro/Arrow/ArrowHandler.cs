using Neuro.DependencyInjection;
using UnityEngine;

namespace Neuro.Arrow;

public class ArrowHandler : IArrowHandler
{
    public IContextProvider Context { get; set; }

    public LineRenderer Arrow { get; set; }
}
