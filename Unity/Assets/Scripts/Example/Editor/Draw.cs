using Entia.Nodes;
using Entia.Unity;
using Systems;
using UnityEngine;
using static Entia.Nodes.Node;

namespace Controllers
{
    [CreateAssetMenu(menuName = "Entia/Node Modifiers/Draw")]
    public sealed class Draw : NodeModifier
    {
        public override Node Modify(Node node) => Sequence(node.Name,
            node,
            System<DrawVelocity>()
        ).Flatten(false);
    }
}