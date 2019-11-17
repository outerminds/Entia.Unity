using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;

namespace Nodes
{
    public sealed class Draw : NodeReference
    {
        public override Node Node => Sequence(nameof(Draw),
            System<Systems.DrawVelocity>()
        ).Editor();
    }
}