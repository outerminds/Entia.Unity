using Entia.Core;
using Entia.Modules;
using System;
using UnityEngine;

namespace Entia.Unity
{
    public interface IResourceReference : IReference
    {
        IResource Value { get; set; }
        IResource Raw { get; set; }
        Type Type { get; }

        void Initialize(World world);
        void Dispose();
    }

    public abstract class ResourceReference : MonoBehaviour { }

    [ExecuteInEditMode, DisallowMultipleComponent]
    public abstract class ResourceReference<T> : ResourceReference, IResourceReference where T : struct, IResource
    {
        protected delegate ref TMember Mapper<TMember>(ref T resource);
        protected delegate TMember From<TMember>(ref T resource, World world);
        protected delegate void To<TMember>(ref T resource, TMember value, World world);

        public World World { get; private set; }

        public abstract T Raw { get; set; }
        public T Value
        {
            get
            {
                if (World is World world) return world.Resources().Get<T>();
                return Raw;
            }
            set
            {
                if (World is World world && world.Resources().Has<T>()) world.Resources().Set(value);
                Raw = value;
            }
        }

        Type IResourceReference.Type => typeof(T);
        IResource IResourceReference.Raw { get => Raw; set => Raw = value is T resource ? resource : default; }
        IResource IResourceReference.Value
        {
            get => Value;
            set { if (value is T casted) Value = casted; }
        }

        [NonSerialized]
        bool _initialized;
        [NonSerialized]
        bool _disposed;

        protected ResourceReference() { Raw = DefaultUtility.Default<T>(); }

        protected ref TMember Get<TMember>(Mapper<TMember> map, ref TMember member) => ref
            World is World world && world.Resources().Has<T>() ?
            ref map(ref world.Resources().Get<T>()) : ref member;

        protected TMember Get<TMember>(From<TMember> from, TMember member) =>
            World is World world && world.Resources().Has<T>() ?
            from(ref world.Resources().Get<T>(), world) : member;

        protected void Set<TMember>(To<TMember> set, TMember value, ref TMember member)
        {
            if (World is World world && world.Resources().Has<T>())
                set(ref world.Resources().Get<T>(), value, world);
            else member = value;
        }

        void Awake()
        {
            if (gameObject.TryWorld(out var world)) Initialize(world);
        }

        void OnDestroy() => Dispose();

        void Initialize(World world)
        {
            if (world == null) return;
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