using UnityEngine;

namespace Entia.Experimental.Unity
{
    public abstract class NodeReference : ScriptableObject
    {
        public abstract Node Node { get; }
    }
}