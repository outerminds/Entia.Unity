using Entia.Core;
using Entia.Modules;
using Entia.Modules.Component;
using System;
using UnityEngine;

namespace Entia.Unity
{
    public interface IComponentReference
    {
        World World { get; }
        Entity Entity { get; }
        IComponent Value { get; set; }
        IComponent Raw { get; set; }
        Type Type { get; }
        Metadata Metadata { get; }

        void Initialize(Entity entity, World world);
        void Dispose();
        bool Copy(Entity entity, World world);
        bool Clone(Entity entity, World world);
    }

    [RequireComponent(typeof(EntityReference))]
    public abstract class ComponentReference : MonoBehaviour { }

    [DisallowMultipleComponent]
    public abstract class ComponentReference<T> : ComponentReference, IComponentReference where T : struct, IComponent
    {
        protected delegate ref TMember Mapper<TMember>(ref T component);
        protected delegate TMember From<TMember>(ref T component, World world);
        protected delegate void To<TMember>(ref T component, in TMember value, World world);

        World IComponentReference.World => _world;
        Entity IComponentReference.Entity => _entity;

        protected abstract T Raw { get; set; }

        IComponent IComponentReference.Value
        {
            get
            {
                if (_initialized && !_disposed && _world.Components().TryGet<T>(_entity, out var component)) return component;
                return Raw;
            }
            set
            {
                if (value is T casted)
                {
                    if (_initialized && !_disposed && _world.Components().Has<T>(_entity)) _world.Components().Set(_entity, casted);
                    else Raw = casted;
                }
            }
        }
        IComponent IComponentReference.Raw { get => Raw; set => Raw = value is T component ? component : default; }
        Type IComponentReference.Type => typeof(T);
        Metadata IComponentReference.Metadata => ComponentUtility.Concrete<T>.Data;

        protected World _world;
        protected Entity _entity;
        bool _initialized;
        bool _disposed;

        protected ComponentReference() { Raw = DefaultUtility.Default<T>(); }

        public bool Copy(Entity entity, World world) => world.Components().Set(entity, Raw);

        public bool Clone(Entity entity, World world)
        {
            if (TypeUtility.Cache<T>.Data.IsPlain) return Copy(entity, world);
            return world.Cloners().Clone(Raw).TryValue(out var component) && world.Components().Set(entity, component);
        }

        protected ref TMember Get<TMember>(Mapper<TMember> map, ref TMember member) => ref
            _world is World world && world.Components().Has<T>(_entity) ?
            ref map(ref world.Components().Get<T>(_entity)) : ref member;

        protected TMember Get<TMember>(From<TMember> from, in TMember member) =>
            _world is World world && world.Components().Has<T>(_entity) ?
            from(ref world.Components().Get<T>(_entity), world) : member;

        protected void Set<TMember>(To<TMember> set, in TMember value, ref TMember member)
        {
            if (_world is World world && world.Components().Has<T>(_entity))
                set(ref world.Components().Get<T>(_entity), value, world);
            else member = value;
        }

        void Awake()
        {
            if (GetComponent<IEntityReference>() is IEntityReference reference && reference.Entity && reference.World is World world)
                Initialize(reference.Entity, world);
        }
        void OnDestroy() => Dispose();

        void Initialize(Entity entity, World world)
        {
            if (entity == Entity.Zero || world == null) return;
            if (_initialized.Change(true))
            {
                _world = world;
                _entity = entity;
                _world.Components().Set(_entity, Raw);
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                _world.Components().Remove<T>(_entity);
                _entity = Entity.Zero;
                _world = null;
            }
        }

        void IComponentReference.Initialize(Entity entity, World world) => Initialize(entity, world);
        void IComponentReference.Dispose() => Dispose();
    }
}