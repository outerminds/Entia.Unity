using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;

namespace Controllers
{
    public class Main : ControllerReference
    {
        // This 'Node' represents the execution behavior of systems.
        public override Node Node =>
            // The 'Sequence' node executes its children in order.
            Sequence("Main",
                // This node holds a few useful Unity-specific library systems.
                Nodes.Default,
                // Any number of systems can be added here.
                System<Systems.UpdateInput>(),
                System<Systems.UpdateVelocity>(),
                System<Systems.UpdatePosition>()
            );
    }
}