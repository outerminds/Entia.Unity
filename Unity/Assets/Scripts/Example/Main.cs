using Entia.Nodes;
using Entia.Unity;
using static Entia.Unity.Nodes;
using static Entia.Nodes.Node;

namespace Nodes
{
    public sealed class Main : NodeReference
    {
        // This 'Node' represents the execution behavior of systems.
        // The 'Sequence' node executes its children in order.
        public override Node Node => Sequence(nameof(Main),
            // This node holds a few useful Unity-specific library systems.
            Default,
            // Any number of systems can be added here.
            System<Systems.UpdateInput>(),
            System<Systems.UpdateVelocity>(),
            System<Systems.UpdatePosition>()
        );
    }
}