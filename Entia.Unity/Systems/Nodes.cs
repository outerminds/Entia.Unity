using Entia.Nodes;
using static Entia.Nodes.Node;

namespace Entia.Unity
{
    public static class Nodes
    {
        public static readonly Node Default =
            Sequence(nameof(Unity),
                System<Systems.LogException>(),
                System<Systems.SynchronizeEntityReference>(),
                System<Systems.UpdateTime>()
            );
    }
}
