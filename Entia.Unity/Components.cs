using Entia.Core;
using Entia.Delegables;
using Entia.Dependables;
using Entia.Unity;
using UnityEngine;

namespace Entia.Components
{
    public readonly struct IsDisabled : IEnabled { }

    public struct Unity<T> : IEnabled, IDelegable<Delegates.Unity<T>>, IDependable<Dependers.Unity<T>> where T : UnityEngine.Object
    {
        public T Value;
    }
}