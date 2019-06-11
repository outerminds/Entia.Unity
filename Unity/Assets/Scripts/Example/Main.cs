using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Unity.Nodes;
using static Entia.Nodes.Node;
using UnityEngine;

namespace Nodes
{
    public sealed class Main : NodeReference
    {
        // This 'Node' represents the execution behavior of systems.
        public override Node Node =>
            // The 'Sequence' node executes its children in order.
            Sequence(name,
                // This node holds a few useful Unity-specific library systems.
                Default,
                // Any number of systems can be added here.
                System<Systems.UpdateInput>(),
                System<Systems.UpdateVelocity>(),
                System<Systems.UpdatePosition>()
            );
    }
}