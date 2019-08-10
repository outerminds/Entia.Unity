using System.Collections.Generic;
using System.Linq;
using Entia.Dependencies;
using Entia.Dependency;
using UnityEngine;

namespace Entia.Dependers
{
    public sealed class Unity : IDepender
    {
        public IEnumerable<IDependency> Depend(in Context context) => new IDependency[] { new Dependencies.Unity() };
    }

    public sealed class Unity<T> : IDepender where T : Object
    {
        public IEnumerable<IDependency> Depend(in Context context) =>
            context.Dependencies<T>().Prepend(new Dependencies.Unity());
    }
}