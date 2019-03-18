using System.Collections.Generic;
using Entia.Core;
using Entia.Dependers;
using Entia.Injectables;
using Entia.Injectors;
using Entia.Messages;
using Entia.Modules;
using Entia.Unity;
using UnityEngine;

namespace Entia.Injectables
{
    public readonly struct Cloner : IInjectable
    {
        [Injector]
        static readonly Injector<Cloner> _injector = Injector.From(world => new Cloner(world));
        [Depender]
        static readonly IDepender _depender = Depender.From(
            new Dependencies.Unity(),
            new Dependencies.Write(typeof(Entity)),
            new Dependencies.Write(typeof(IComponent)),
            new Dependencies.Emit(typeof(OnCreate)),
            new Dependencies.Emit(typeof(OnAdd)),
            new Dependencies.Emit(typeof(OnAdd<>)));

        readonly World _world;
        readonly Modules.Entities _entities;
        readonly Modules.Components _components;
        readonly Modules.Delegates _delegates;
        readonly Pool<List<ComponentReference>> _references;

        public Cloner(World world)
        {
            _world = world;
            _entities = world.Entities();
            _components = world.Components();
            _delegates = world.Delegates();
            _references = new Pool<List<ComponentReference>>(
                () => new List<ComponentReference>(),
                instance => instance.Clear());
        }

        public Entity Clone(Entity source)
        {
            var target = _entities.Create();
            _components.Copy(source, target);
            return target;
        }

        public Entity Clone(EntityReference source, bool pool = true)
        {
            var target = _entities.Create();
            if (Debug.isDebugBuild) _components.Set(target, new Components.Debug { Name = source.Name });

            var cloned = false;
            using (var list = _references.Use())
            {
                source.GetComponents(list.Instance);
                foreach (var component in list.Instance) cloned |= _delegates.Get(component.GetType()).Set(component, target, _world);
            }

            if (pool) _components.Set(target, new Components.Pooled { Reference = source });
            return target;
        }
    }
}