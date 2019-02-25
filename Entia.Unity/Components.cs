using Entia.Core;
using Entia.Dependables;
using Entia.Initializers;
using Entia.Instantiators;
using Entia.Modules.Template;
using Entia.Templaters;
using Entia.Unity;
using UnityEngine;

namespace Entia.Components
{
    public struct IsDisabled : IComponent { }
    public struct Debug : IComponent { public string Name; }

    [Plain]
    public struct Unity<T> : IComponent, IComponentDelegable<ComponentDelegate<T>>, IDependable<Dependers.Unity<T>> where T : UnityEngine.Object
    {
        sealed class Templater : Templater<Unity<T>>
        {
            public override Result<IInitializer> Initializer(Components.Unity<T> value, Context context, World world) => new Initializers.Identity();
            public override Result<IInstantiator> Instantiator(Components.Unity<T> value, Context context, World world) => new Clone(value);
        }

        [Templater]
        static readonly Templater _templater = new Templater();

        public T Value;
    }
}