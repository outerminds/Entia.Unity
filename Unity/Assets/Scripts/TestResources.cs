using Entia;
using Entia.Unity;
using UnityEngine;

namespace Resources
{
    public struct Resource1 : IResource { }

    namespace Inner
    {
        public struct Resource2 : IResource { }
    }

    public struct Prefabs : IResource
    {
        public EntityReference Prefab;
    }
}