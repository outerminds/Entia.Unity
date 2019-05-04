using Entia.Unity;
using UnityEngine.SceneManagement;

namespace Entia.Resources
{
    public struct Time : IResource
    {
        [Disable]
        public float Delta;
        [Disable]
        public float Current;
    }

    public struct Unity : IResource
    {
        public Scene Scene;
        public WorldReference Reference;
    }
}
