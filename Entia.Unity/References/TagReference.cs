using Entia.Core;
using Entia.Modules;
using System;
using UnityEngine;

namespace Entia.Unity
{
    public interface ITagReference
    {
        World World { get; }
        Entity Entity { get; }
        Type Type { get; }

        void Initialize(Entity entity, World world);
        void Dispose();
    }

    [RequireComponent(typeof(EntityReference))]
    public abstract class TagReference : MonoBehaviour { }

    [DisallowMultipleComponent]
    public abstract class TagReference<T> : TagReference, ITagReference where T : struct, ITag
    {
        public World World { get; private set; }
        public Entity Entity { get; private set; }

        Type ITagReference.Type => typeof(T);

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
        void Add() => World?.Tags().Set<T>(Entity);
        void Remove() => World?.Tags().Remove<T>(Entity);

        void Initialize(Entity entity, World world)
        {
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

        void ITagReference.Initialize(Entity entity, World world) => Initialize(entity, world);
        void ITagReference.Dispose() => Dispose();
    }
}