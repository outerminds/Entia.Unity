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
        public abstract T Component { get; set; }

        Type IComponentReference.Type => typeof(T);
        Metadata IComponentReference.Metadata => ComponentUtility.Cache<T>.Data;
        IComponent IComponentReference.Component { get => Component; set => Component = value is T component ? component : default; }

        bool _initialized;
        bool _disposed;

        void Awake()
        {
            if (GetComponent<IEntityReference>() is IEntityReference reference && reference.World is World world)
                Initialize(reference.Entity, world);
        }
        void OnDestroy() => Dispose();
        void OnEnable() => Add();
        void OnDisable() => Remove();
        void Add() => World?.Components().Set(Entity, Component);
        void Remove() => World?.Components().Remove<T>(Entity);

        void Initialize(Entity entity, World world)
        {
            if (entity == Entity.Zero || world == null) return;
            if (_initialized.Change(true))
            {
                World = world;
                Entity = entity;
                Add();
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                Remove();
                Entity = Entity.Zero;
                World = null;
            }
        }

        void IComponentReference.Initialize(Entity entity, World world) => Initialize(entity, world);
        void IComponentReference.Dispose() => Dispose();
    }
}