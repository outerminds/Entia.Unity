using Entia.Unity;

namespace Entia.Resources
{
    public struct Time : IResource
    {
        [Disable]
        public float Delta;
        [Disable]
        public float Current;
    }
}
