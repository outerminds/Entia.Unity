using System;
using System.Collections.Generic;
using Entia.Components;
using Entia.Core;
using Entia.Modules;
using UnityEngine;

namespace Entia.Unity
{
    public interface IComponentDelegable<T> where T : IComponentDelegate, new() { }
    public interface IComponentDelegate
    {
        Type Type { get; }
        bool TryCreate(UnityEngine.Object value, out IComponent component);
        bool TryGet(Entity entity, World world, out IComponent component);
        bool Set(UnityEngine.Object value, Entity entity, World world);
        bool Remove(Entity entity, World world);
    }

    public static class ComponentDelegate
    {
        static readonly TypeMap<UnityEngine.Object, IComponentDelegate> _modifiers = new TypeMap<UnityEngine.Object, IComponentDelegate>();

        public static bool TryGet(Type type, out IComponentDelegate modifier)
        {
            if (_modifiers.TryGet(type, out modifier)) return modifier != null;
            try { modifier = Activator.CreateInstance(typeof(ComponentDelegate<>).MakeGenericType(type)) as IComponentDelegate; }
            catch { }
            return (_modifiers[type] = modifier) != null;
        }

        public static IComponentDelegate Get<T>() where T : UnityEngine.Object
        {
            if (_modifiers.TryGet<T>(out var modifier)) return modifier;
            _modifiers.Set<T>(modifier = new ComponentDelegate<T>());
            return modifier;
        }
    }

    public sealed class ComponentDelegate<T> : IComponentDelegate where T : UnityEngine.Object
    {
        public Type Type => typeof(Unity<T>);

        public bool TryCreate(UnityEngine.Object value, out IComponent component)
        {
            if (value is T casted)
            {
                component = new Unity<T> { Value = casted };
                return true;
            }

            component = default;
            return false;
        }

        public bool TryGet(Entity entity, World world, out IComponent component)
        {
            if (world.Components().TryGet<Unity<T>>(entity, out var unity))
            {
                component = unity;
                return true;
            }

            component = default;
            return false;
        }

        public bool Set(UnityEngine.Object value, Entity entity, World world) => value is T casted && world.Components().Set(entity, new Unity<T> { Value = casted });
        public bool Remove(Entity entity, World world) => world.Components().Remove<Unity<T>>(entity);
    }
}