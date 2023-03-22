using Neuro.DependencyInjection;
using UnityEngine;

namespace Neuro.Arrow;

// TODO: Is dependency injection really necessary for this?
public interface IArrowHandler : IContextAcceptor
{
    public LineRenderer Arrow { get; set; }
}
