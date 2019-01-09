using Entia.Nodes;
using static Entia.Nodes.Node;

namespace Entia.Unity
{
    public static class Nodes
    {
        public static readonly Node Default =
            Sequence("Unity",
                System<Systems.LogException>(),
                System<Systems.Time>(),
                System<Systems.SynchronizeGameObject>()
            );
    }
}
