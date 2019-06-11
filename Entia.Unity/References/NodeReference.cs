using Entia.Nodes;
using UnityEngine;

namespace Entia.Unity
{
    public interface INodeReference
    {
        Node Node { get; }
    }

    public abstract class NodeReference : ScriptableObject, INodeReference
    {
        public abstract Node Node { get; }
    }
}