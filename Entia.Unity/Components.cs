using Entia.Core;

namespace Entia.Components
{
    public readonly struct IsDisabled : IEnabled { }

    public struct Unity<T> : IEnabled, IImplementation<Dependers.Unity<T>> where T : UnityEngine.Object
    {
        public T Value;
    }
}