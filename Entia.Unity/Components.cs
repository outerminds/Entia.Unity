using Entia.Delegables;
using Entia.Dependables;

namespace Entia.Components
{
    public readonly struct IsDisabled : IEnabled { }

    public struct Unity<T> : IEnabled, IDelegable<Delegates.Unity<T>>, IDependable<Dependers.Unity<T>> where T : UnityEngine.Object
    {
        public T Value;
    }
}