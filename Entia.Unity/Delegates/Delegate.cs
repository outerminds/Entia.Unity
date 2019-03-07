using System;
using Entia.Core;
using Entia.Modules;
using Entia.Modules.Component;
using Entia.Unity;
using UnityEngine;

namespace Entia.Delegates
{
    public interface IDelegate
    {
        Metadata Type { get; }
        bool TryCreate(UnityEngine.Object value, out IComponent component);
        bool TryGet(Entity entity, World world, out IComponent component);
        bool Set(UnityEngine.Object value, Entity entity, World world);
        bool Remove(UnityEngine.Object value, Entity entity, World world);
        bool Clone(UnityEngine.Object value, Entity entity, Depth depth, World world);
    }

    public abstract class Delegate<T> : IDelegate where T : UnityEngine.Object
    {
        public abstract Metadata Type { get; }

        public abstract bool TryCreate(T value, out IComponent component);
        public abstract bool TryGet(Entity entity, World world, out IComponent component);
        public abstract bool Set(T value, Entity entity, World world);
        public abstract bool Remove(T value, Entity entity, World world);
        public abstract bool Clone(T value, Entity entity, Depth depth, World world);

        bool IDelegate.TryCreate(UnityEngine.Object value, out IComponent component)
        {
            if (value is T casted) return TryCreate(casted, out component);
            component = default;
            return false;
        }

        bool IDelegate.Set(UnityEngine.Object value, Entity entity, World world) => value is T casted && Set(casted, entity, world);
        bool IDelegate.Remove(UnityEngine.Object value, Entity entity, World world) => value is T casted && Remove(casted, entity, world);
        bool IDelegate.Clone(UnityEngine.Object value, Entity entity, Depth depth, World world) => value is T casted && Clone(casted, entity, depth, world);
    }

    public sealed class Default : IDelegate
    {
        public Metadata Type => default;

        public bool TryCreate(UnityEngine.Object value, out IComponent component)
        {
            component = default;
            return false;
        }

        public bool TryGet(Entity entity, World world, out IComponent component)
        {
            component = default;
            return false;
        }

        public bool Set(UnityEngine.Object value, Entity entity, World world) => false;
        public bool Remove(UnityEngine.Object value, Entity entity, World world) => false;
        public bool Clone(UnityEngine.Object value, Entity entity, Depth depth, World world) => false;
    }

    public sealed class Reference<T> : Delegate<ComponentReference<T>> where T : struct, IComponent
    {
        public override Metadata Type => ComponentUtility.Concrete<T>.Data;

        public override bool TryCreate(ComponentReference<T> value, out IComponent component)
        {
            component = value.Raw;
            return true;
        }

        public override bool TryGet(Entity entity, World world, out IComponent component)
        {
            if (world.Components().TryGet<T>(entity, out var casted))
            {
                component = casted;
                return true;
            }

            component = default;
            return false;
        }

        public override bool Set(ComponentReference<T> value, Entity entity, World world) => world.Components().Set(entity, value.Raw);
        public override bool Remove(ComponentReference<T> value, Entity entity, World world) => world.Components().Remove<T>(entity);

        public override bool Clone(ComponentReference<T> value, Entity entity, Depth depth, World world)
        {
            if (depth == Depth.Shallow || Type.Data.IsPlain) return Set(value, entity, world);
            return world.Cloners().Clone(value.Raw).TryValue(out var clone) && world.Components().Set(entity, clone);
        }
    }

    public sealed class Unity<T> : Delegate<T> where T : UnityEngine.Object
    {
        public override Metadata Type => ComponentUtility.Concrete<Components.Unity<T>>.Data;

        public override bool TryCreate(T value, out IComponent component)
        {
            component = new Components.Unity<T> { Value = value };
            return true;
        }

        public override bool TryGet(Entity entity, World world, out IComponent component)
        {
            if (world.Components().TryGet<Components.Unity<T>>(entity, out var unity))
            {
                component = unity;
                return true;
            }

            component = default;
            return false;
        }

        public override bool Set(T value, Entity entity, World world) => world.Components().Set(entity, new Components.Unity<T> { Value = value });

        public override bool Remove(T value, Entity entity, World world) =>
            world.Components().TryUnity<T>(entity, out var component) &&
            value == component &&
            world.Components().Remove<Components.Unity<T>>(entity);

        public override bool Clone(T value, Entity entity, Depth depth, World world) => Set(value, entity, world);
    }
}