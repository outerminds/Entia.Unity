using System.Collections.Generic;
using System.Reflection;
using Entia.Dependencies;
using Entia.Dependers;
using Entia.Modules;
using UnityEngine;

namespace Entia.Dependers
{
    public sealed class Unity : IDepender
    {
        public IEnumerable<IDependency> Depend(MemberInfo member, World world)
        {
            yield return new Dependencies.Unity();
        }
    }

    public sealed class Unity<T> : IDepender where T : Object
    {
        public IEnumerable<IDependency> Depend(MemberInfo member, World world)
        {
            yield return new Dependencies.Unity();
            foreach (var dependency in world.Dependers().Dependencies<T>()) yield return dependency;
        }
    }
}