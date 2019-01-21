using Entia.Dependables;
using UnityEngine;

namespace Entia.Components
{
    public struct IsDisabled : IComponent { }
    public struct Debug : IComponent { public string Name; }
    public struct Unity<T> : IComponent, IDependable<Dependers.Unity<T>> where T : Object { public T Value; }
}