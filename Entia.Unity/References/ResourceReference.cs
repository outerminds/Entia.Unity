using Entia.Core;
using Entia.Modules;
using System;
using UnityEngine;

namespace Entia.Unity
{
    public interface IResourceReference
    {
        World World { get; }
        IResource Resource { get; set; }
        Type Type { get; }

        void Initialize(World world);
        void Dispose();
    }

    public abstract class ResourceReference : MonoBehaviour { }

    [DisallowMultipleComponent]
    public abstract class ResourceReference<T> : ResourceReference, IResourceReference where T : struct, IResource
    {
        public World World { get; private set; }
        public abstract T Resource { get; set; }

        IResource IResourceReference.Resource { get => Resource; set => Resource = value is T casted ? casted : default; }
        Type IResourceReference.Type => typeof(T);

        bool _initialized;
        bool _disposed;

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
                World.Resources().Set(Resource);
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                World?.Resources().Remove<T>();
                World = null;
            }
        }

        void IResourceReference.Initialize(World world) => Initialize(world);
        void IResourceReference.Dispose() => Dispose();
    }
}