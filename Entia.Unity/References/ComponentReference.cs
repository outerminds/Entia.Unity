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
        IComponent Component { get; set; }
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
        public World World { get; private set; }
        public Entity Entity { get; private set; }
        public T Component
        {
            get
            {
                if (_initialized && !_disposed && World.Components().TryGet<T>(Entity, out var component)) return component;
                else return Raw;
            }
            set
            {
                if (_initialized && !_disposed && World.Components().Has<T>(Entity)) World.Components().Set(Entity, value);
                else Raw = value;
            }
        }
        public abstract T Raw { get; set; }

        IComponent IComponentReference.Component { get => Component; set => Component = value is T component ? component : default; }
        IComponent IComponentReference.Raw { get => Raw; set => Raw = value is T component ? component : default; }
        Type IComponentReference.Type => typeof(T);
        Metadata IComponentReference.Metadata => ComponentUtility.Concrete<T>.Data;

        bool _initialized;
        bool _disposed;

        protected ComponentReference() { Raw = DefaultUtility.Default<T>(); }

        protected virtual void Reset() { }

        void Awake()
        {
            if (GetComponent<IEntityReference>() is IEntityReference reference && reference.World is World world)
                Initialize(reference.Entity, world);
        }
        void OnDestroy() => Dispose();

        void Initialize(Entity entity, World world)
        {
            if (entity == Entity.Zero || world == null) return;
            if (_initialized.Change(true))
            {
                World = world;
                Entity = entity;
                World.Components().Set(Entity, Raw);
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                World.Components().Remove<T>(Entity);
                Entity = Entity.Zero;
                World = null;
            }
        }

        void IComponentReference.Initialize(Entity entity, World world) => Initialize(entity, world);
        void IComponentReference.Dispose() => Dispose();
    }
}