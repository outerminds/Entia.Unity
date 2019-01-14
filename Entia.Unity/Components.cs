using Entia.Dependables;
using UnityEngine;

namespace Entia.Unity.Components
{
    public struct IsDisabled : IComponent { }
    public struct Debug : IComponent { public string Name; }
    public struct Unity<T> : IComponent, IDepend<Dependencies.Unity, T> where T : Object { public T Value; }
}