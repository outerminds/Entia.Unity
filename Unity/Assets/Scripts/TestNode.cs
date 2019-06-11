using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Unity.Nodes;
using static Entia.Nodes.Node;

namespace Nodes
{
    public sealed class TestNode : NodeReference
    {
        public override Node Node =>
            Sequence("TestController",
                Default,
                System<Systems.Move>(),
                // System<Systems.Emitter>(),
                // System<Systems.EmitterA>(),
                System<Systems.Spawner>()
            // System<Systems.Queries>(),
            // System<Systems.Reactive>(),

            // System<Systems.Reactor1>(),
            // System<Systems.Reactor2>(),
            // System<Systems.Reactor3>(),
            // System<Systems.Move>()
            );
    }
}