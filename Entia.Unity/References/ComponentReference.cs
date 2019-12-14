using Entia.Core;
using Entia.Modules;
using Entia.Modules.Component;
using System;
using UnityEngine;

namespace Entia.Unity
{
    public interface IComponentReference : IReference
    {
        Entity Entity { get; }
        IComponent Value { get; set; }
        IComponent Raw { get; set; }
        Type Type { get; }
        Metadata Metadata { get; }

        void Initialize(Entity entity, World world);
        void Dispose();
    }

    [RequireComponent(typeof(EntityReference))]
    public abstract class ComponentReference : MonoBehaviour { }

    [DisallowMultipleComponent]
    public abstract class ComponentReference<T> : ComponentReference, IComponentReference where T : struct, IComponent
    {
        protected delegate TMember From<TMember>(ref T component, World world);
        protected delegate void To<TMember>(ref T component, TMember value, World world);

        public World World { get; private set; }
        public Entity Entity { get; private set; }

        public abstract T Raw { get; set; }

        IComponent IComponentReference.Value
        {
            get
            {
                if (_initialized && !_disposed && World.Components().TryGet<T>(Entity, out var component)) return component;
                return Raw;
            }
            set
            {
                if (value is T casted)
                {
                    if (_initialized && !_disposed && World.Components().Has<T>(Entity)) World.Components().Set(Entity, casted);
                    else Raw = casted;
                }
            }
        }
        IComponent IComponentReference.Raw { get => Raw; set => Raw = value is T component ? component : default; }
        Type IComponentReference.Type => typeof(T);
        Metadata IComponentReference.Metadata => ComponentUtility.Concrete<T>.Data;

        [NonSerialized]
        bool _initialized;
        [NonSerialized]
        bool _disposed;

        protected ComponentReference() { Raw = DefaultUtility.Default<T>(); }

        protected TMember Get<TMember>(From<TMember> from, TMember member) =>
            World is World world && world.Components().Has<T>(Entity) ?
            from(ref world.Components().Get<T>(Entity), world) : member;

        protected void Set<TMember>(To<TMember> set, TMember value, ref TMember member)
        {
            if (World is World world && world.Components().Has<T>(Entity))
                set(ref world.Components().Get<T>(Entity), value, world);
            member = value;
        }

        protected virtual void Awake()
        {
            if (GetComponent<IEntityReference>() is IEntityReference reference && reference.Entity && reference.World is World world)
                Initialize(reference.Entity, world);
        }

        protected virtual void OnDestroy() => Dispose();
        protected virtual void OnEnable() => World?.Components().Enable<T>(Entity);
        protected virtual void OnDisable() => World?.Components().Disable<T>(Entity);

        void Initialize(Entity entity, World world)
        {
            if (entity == Entia.Entity.Zero || world == null) return;
            if (!Application.isPlaying || _initialized.Change(true))
            {
                World = world;
                Entity = entity;
                World.Components().Set(Entity, Raw);

                if (enabled && gameObject.activeInHierarchy) OnEnable();
                else OnDisable();
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                World.Components().Remove<T>(Entity);
                Entity = Entity.Zero;
                World = null;
                _initialized = false;
                _disposed = false;
            }
        }

        void IComponentReference.Initialize(Entity entity, World world) => Initialize(entity, world);
        void IComponentReference.Dispose() => Dispose();
    }
}