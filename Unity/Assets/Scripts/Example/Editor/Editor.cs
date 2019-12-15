using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;

namespace Nodes
{
    public sealed class Editor : NodeReference
    {
        public override Node Node => Sequence(nameof(Editor),
            System<Systems.DrawVelocity>().Editor(),
            System<Systems.SynchronizePosition>().Editor(true)
        );
    }
}