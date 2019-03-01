using Entia.Core;
using Entia.Modules;
using Entia.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Entia.Unity
{
    public interface IEntityReference
    {
        World World { get; }
        Entity Entity { get; }

        Entity Clone(World world);
        Entity Copy(World world);

        void PreInitialize();
        void Initialize(World world, bool propagate = true);
        void PostInitialize();

        void PreDispose();
        void Dispose();
        void PostDispose();
    }

    [DisallowMultipleComponent]
    public sealed class EntityReference : MonoBehaviour, IEntityReference
    {
        [System.Flags]
        enum States : byte
        {
            None = 0,
            All = Pre | Current | Post,
            Pre = 1 << 0,
            Current = 1 << 1,
            Post = 1 << 2
        }

        static readonly List<IEntityReference> _entities = new List<IEntityReference>();
        static readonly List<Component> _components = new List<Component>();

        public World World { get; private set; }
        public Entity Entity { get; private set; }

        States _initialized;
        States _disposed;
        IComponentReference[] _references;
        string _name;

        public Entity Copy(World world)
        {
            var pair = UnpackCache();
            var entity = world.Entities().Create();
            if (UnityEngine.Debug.isDebugBuild) world.Components().Set(entity, new Components.Debug { Name = pair.name });
            for (int i = 0; i < pair.references.Length; i++) pair.references[i].Copy(entity, world);
            return entity;
        }

        public Entity Clone(World world)
        {
            var pair = UnpackCache();
            var entity = world.Entities().Create();
            if (UnityEngine.Debug.isDebugBuild) world.Components().Set(entity, new Components.Debug { Name = pair.name });
            for (int i = 0; i < pair.references.Length; i++) pair.references[i].Clone(entity, world);
            return entity;
        }

        void OnEnable() => World?.Components().Remove<IsDisabled>(Entity);
        void OnDisable() => World?.Components().Set<IsDisabled>(Entity, default);

        void Awake()
        {
            if (WorldRegistry.TryGet(gameObject.scene, out var reference) && reference.World is World world)
            {
                PreInitialize();
                Initialize(world, true);
                PostInitialize();
            }
        }

        void OnDestroy()
        {
            PreDispose();
            Dispose();
            PostDispose();
        }

        void PreInitialize()
        {
            _initialized.Change(_initialized | States.Pre);
        }

        void Initialize(World world, bool propagate = true)
        {
            if (_initialized.Change(_initialized | States.Current))
            {
                World = world;
                Entity = World.Entities().Create();
                EntityRegistry.Set(this);

                if (propagate)
                {
                    GetComponentsInChildren(_entities);
                    foreach (var entity in _entities) entity.Initialize(world);
                }
            }
        }

        void PostInitialize()
        {
            if (World == null || Entity == Entity.Zero) return;
            if (_initialized.Change(_initialized | States.Post))
            {
                var components = World.Components();

                if (UnityEngine.Debug.isDebugBuild) components.Set(Entity, new Components.Debug { Name = name });
                if (enabled && gameObject.activeInHierarchy) OnEnable();
                else OnDisable();

                components.Set(Entity, new Components.Unity<UnityEngine.GameObject> { Value = gameObject });
                GetComponents(_components);
                foreach (var component in _components)
                {
                    switch (component)
                    {
                        case IEntityReference _: break;
                        case IComponentReference reference: reference.Initialize(Entity, World); break;
                        default:
                            if (ComponentDelegate.TryGet(component.GetType(), out var @delegate))
                                @delegate.Set(component, Entity, World);
                            break;
                    }
                }
                _components.Clear();
            }
        }

        void PreDispose()
        {
            if (_initialized == States.All && _disposed.Change(_disposed | States.Pre))
            {
                var components = World.Components();

                GetComponents(_components);
                foreach (var component in _components)
                {
                    switch (component)
                    {
                        case IEntityReference _: break;
                        case IComponentReference reference: reference.Dispose(); break;
                        default:
                            if (ComponentDelegate.TryGet(component.GetType(), out var @delegate))
                                @delegate.Remove(component, Entity, World);
                            break;
                    }
                }
                _components.Clear();

                components.Remove<Components.Unity<UnityEngine.GameObject>>(Entity);
                if (UnityEngine.Debug.isDebugBuild) components.Remove<Components.Debug>(Entity);
            }
        }

        void Dispose()
        {
            if (_initialized == States.All && _disposed.Change(_disposed | States.Current))
            {
                EntityRegistry.Remove(this);
                World?.Entities().Destroy(Entity);
                Entity = Entity.Zero;
                World = null;
            }
        }

        void PostDispose()
        {
            if (_initialized == States.All && _disposed.Change(_disposed | States.Post))
            {
                _initialized = States.None;
                _disposed = States.None;
            }
        }

        (string name, IComponentReference[] references) UnpackCache() => (
            string.IsNullOrEmpty(_name) ? (_name = name) : _name,
            _references ?? (_references = GetComponents<IComponentReference>()));

        void IEntityReference.PreInitialize() => PreInitialize();
        void IEntityReference.Initialize(World world, bool propagate) => Initialize(world, propagate);
        void IEntityReference.PostInitialize() => PostInitialize();
        void IEntityReference.PreDispose() => PreDispose();
        void IEntityReference.Dispose() => Dispose();
        void IEntityReference.PostDispose() => PostDispose();
    }
}