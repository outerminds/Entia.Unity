using Entia.Nodes;
using UnityEngine;

namespace Entia.Unity
{
    public interface INodeModifier
    {
        Node Modify(Node node);
    }

    public abstract class NodeModifier : ScriptableObject, INodeModifier
    {
        public abstract Node Modify(Node node);
    }

    [CreateAssetMenu(menuName = "Entia/Node Modifiers/Profile", fileName = "ProfileModifier")]
    public sealed class ProfileModifier : NodeModifier
    {
        public override Node Modify(Node node) => Debug.isDebugBuild ? node.Profile() : node;
    }
}