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
        bool Remove(UnityEngine.Object value, Entity entity, World world);
    }

    public static class ComponentDelegate
    {
        static readonly TypeMap<UnityEngine.Object, IComponentDelegate> _delegates = new TypeMap<UnityEngine.Object, IComponentDelegate>();

        public static bool TryGet(Type type, out IComponentDelegate @delegate)
        {
            if (_delegates.TryGet(type, out @delegate)) return @delegate != null;
            try { @delegate = Activator.CreateInstance(typeof(ComponentDelegate<>).MakeGenericType(type)) as IComponentDelegate; }
            catch { }
            return (_delegates[type] = @delegate) != null;
        }

        public static IComponentDelegate Get<T>() where T : UnityEngine.Object
        {
            if (_delegates.TryGet<T>(out var @delegate)) return @delegate;
            _delegates.Set<T>(@delegate = new ComponentDelegate<T>());
            return @delegate;
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
        public bool Remove(UnityEngine.Object value, Entity entity, World world) =>
            value is T casted && world.Components().TryUnity<T>(entity, out var component) &&
            casted == component && world.Components().Remove<Unity<T>>(entity);
    }
}