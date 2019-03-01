using Entia.Core;
using Entia.Modules;
using System;
using UnityEngine;

namespace Entia.Unity
{
    public interface IResourceReference
    {
        World World { get; }
        IResource Value { get; set; }
        IResource Raw { get; set; }
        Type Type { get; }

        void Initialize(World world);
        void Dispose();
    }

    public abstract class ResourceReference : MonoBehaviour { }

    [DisallowMultipleComponent]
    public abstract class ResourceReference<T> : ResourceReference, IResourceReference where T : struct, IResource
    {
        protected delegate ref TMember Mapper<TMember>(ref T resource);
        protected delegate TMember From<TMember>(ref T resource, World world);
        protected delegate void To<TMember>(ref T resource, in TMember value, World world);

        public World World { get; private set; }

        protected abstract T Raw { get; set; }

        IResource IResourceReference.Value
        {
            get
            {
                if (_initialized && !_disposed) return World.Resources().Get<T>();
                return Raw;
            }
            set
            {
                if (value is T casted)
                {
                    if (_initialized && !_disposed) World.Resources().Set(casted);
                    else Raw = casted;
                }
            }
        }
        IResource IResourceReference.Raw { get => Raw; set => Raw = value is T resource ? resource : default; }
        Type IResourceReference.Type => typeof(T);

        bool _initialized;
        bool _disposed;

        protected ResourceReference() { Raw = DefaultUtility.Default<T>(); }

        protected ref TMember Get<TMember>(Mapper<TMember> map, ref TMember member) => ref
            World is World world && world.Resources().Has<T>() ?
            ref map(ref world.Resources().Get<T>()) : ref member;

        protected TMember Get<TMember>(From<TMember> from, in TMember member) =>
            World is World world && world.Resources().Has<T>() ?
            from(ref world.Resources().Get<T>(), world) : member;

        protected void Set<TMember>(To<TMember> set, in TMember value, ref TMember member)
        {
            if (World is World world && world.Resources().Has<T>())
                set(ref world.Resources().Get<T>(), value, world);
            else member = value;
        }

        void Awake()
        {
            if (WorldRegistry.TryGet(gameObject.scene, out var reference) && reference.World is World world)
                Initialize(world);
        }

        void OnDestroy() => Dispose();

        void Initialize(World world)
        {
            if (_initialized.Change(true))
            {
                World = world;
                World.Resources().Set(Raw);
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                World?.Resources().Remove<T>();
                World = null;
                _initialized = false;
                _disposed = false;
            }
        }

        void IResourceReference.Initialize(World world) => Initialize(world);
        void IResourceReference.Dispose() => Dispose();
    }
}