using Entia.Nodes;
using Entia.Unity;
using UnityEngine;
using static Entia.Nodes.Node;

namespace Nodes
{
    public sealed class Draw : NodeReference
    {
        public override Node Node => Sequence(
            System<Systems.DrawVelocity>()
        );
    }
}