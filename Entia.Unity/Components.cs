using Entia.Core;
using Entia.Delegables;
using Entia.Dependables;
using Entia.Unity;
using UnityEngine;

namespace Entia.Components
{
    public struct IsDisabled : IComponent { }
    public struct Debug : IComponent { public string Name; }

    [Plain]
    public struct Unity<T> : IComponent, IDelegable<Delegates.Unity<T>>, IDependable<Dependers.Unity<T>> where T : UnityEngine.Object
    {
        public T Value;
    }

    [Plain]
    public struct Link : IComponent
    {
        public EntityReference Reference;
        public GameObject Instance;
    }
}