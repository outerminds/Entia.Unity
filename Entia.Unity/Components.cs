using Entia.Core;
using Entia.Dependables;
using Entia.Unity;
using UnityEngine;

namespace Entia.Components
{
    public struct Debug : IComponent { public string Name; }
    public struct Unity<T> : IComponent, IComponentDelegable<ComponentDelegate<T>>, IDependable<Dependers.Unity<T>> where T : Object
    {
        public T Value;
    }
}