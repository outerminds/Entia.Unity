using Entia.Core;
using Entia.Modules;
using Entia.Modules.Component;
using Entia.Unity;

namespace Entia.Delegates
{
    [Implementation(typeof(UnityEngine.Object), typeof(Unity<>))]
    public interface IDelegate : ITrait
    {
        Metadata Type { get; }
        bool TryCreate(UnityEngine.Object value, out IComponent component);
        bool TryGet(Entity entity, World world, out IComponent component);
        bool Set(UnityEngine.Object value, Entity entity, World world);
        bool Remove(UnityEngine.Object value, Entity entity, World world);
    }

    public abstract class Delegate<T> : IDelegate where T : UnityEngine.Object
    {
        public abstract Metadata Type { get; }

        public abstract bool TryCreate(T value, out IComponent component);
        public abstract bool TryGet(Entity entity, World world, out IComponent component);
        public abstract bool Set(T value, Entity entity, World world);
        public abstract bool Remove(T value, Entity entity, World world);

        bool IDelegate.TryCreate(UnityEngine.Object value, out IComponent component)
        {
            if (value is T casted) return TryCreate(casted, out component);
            component = default;
            return false;
        }

        bool IDelegate.Set(UnityEngine.Object value, Entity entity, World world) => value is T casted && Set(casted, entity, world);
        bool IDelegate.Remove(UnityEngine.Object value, Entity entity, World world) => value is T casted && Remove(casted, entity, world);
    }

    [Preserve]
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
    }
}