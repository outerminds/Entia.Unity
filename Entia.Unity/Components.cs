using Entia.Core;
using Entia.Delegables;

namespace Entia.Components
{
    public readonly struct IsDisabled : IEnabled { }

    public struct Unity<T> : IEnabled, IDelegable<Delegates.Unity<T>>, IImplementation<Dependers.Unity<T>> where T : UnityEngine.Object
    {
        public T Value;
    }
}