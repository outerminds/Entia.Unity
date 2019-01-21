using Entia.Core;
using Entia.Modules;
using Entia.Components;
using UnityEngine;

namespace Entia.Mappers
{
    public interface IMapper<T, TOut>
    {
        TOut Map<TIn>(in TIn input) where TIn : T;
    }

    public abstract class Mapper<T> : IMapper<T, Unit>
    {
        public abstract void Map<TIn>(in TIn input) where TIn : T;
        Unit IMapper<T, Unit>.Map<TIn>(in TIn input) { Map(input); return new Unit(); }
    }

    public readonly struct SetComponent : IMapper<Component, Unit>
    {
        public readonly Entity Entity;
        public readonly Modules.Components Components;

        public SetComponent(Entity entity, Modules.Components components)
        {
            Entity = entity;
            Components = components;
        }

        public Unit Map<TIn>(in TIn input) where TIn : Component
        {
            Components.Set(Entity, new Unity<TIn> { Value = input });
            return default;
        }
    }

    public readonly struct RemoveComponent : IMapper<Component, Unit>
    {
        public readonly Entity Entity;
        public readonly Modules.Components Components;

        public RemoveComponent(Entity entity, Modules.Components components)
        {
            Entity = entity;
            Components = components;
        }

        public Unit Map<TIn>(in TIn input) where TIn : Component
        {
            Components.Remove<Unity<TIn>>(Entity);
            return default;
        }
    }

    public readonly struct ToComponent : IMapper<Component, IComponent>
    {
        public IComponent Map<TIn>(in TIn input) where TIn : Component => new Unity<TIn> { Value = input };
    }
}
