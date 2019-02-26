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
        void Initialize(World world);
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

        public Entity Entity { get; private set; }
        World IEntityReference.World => _world;

        World _world;
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

        void OnEnable() => _world?.Components().Remove<IsDisabled>(Entity);
        void OnDisable() => _world?.Components().Set<IsDisabled>(Entity, default);

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

        void Initialize(World world, bool propagate)
        {
            if (_initialized.Change(_initialized | States.Current))
            {
                _world = world;
                Entity = _world.Entities().Create();
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
            if (_world == null || Entity == Entity.Zero) return;
            if (_initialized.Change(_initialized | States.Post))
            {
                var components = _world.Components();

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
                        case IComponentReference reference: reference.Initialize(Entity, _world); break;
                        default:
                            if (ComponentDelegate.TryGet(component.GetType(), out var @delegate))
                                @delegate.Set(component, Entity, _world);
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
                var components = _world.Components();

                GetComponents(_components);
                foreach (var component in _components)
                {
                    switch (component)
                    {
                        case IEntityReference _: break;
                        case IComponentReference reference: reference.Dispose(); break;
                        default:
                            if (ComponentDelegate.TryGet(component.GetType(), out var @delegate))
                                @delegate.Remove(component, Entity, _world);
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
                _world?.Entities().Destroy(Entity);
                Entity = Entity.Zero;
                _world = null;
            }
        }

        void PostDispose()
        {
            if (_initialized == States.All) _disposed.Change(_disposed | States.Post);
        }

        (string name, IComponentReference[] references) UnpackCache() => (
            string.IsNullOrEmpty(_name) ? (_name = name) : _name,
            _references ?? (_references = GetComponents<IComponentReference>()));

        void IEntityReference.PreInitialize() => PreInitialize();
        void IEntityReference.Initialize(World world) => Initialize(world, false);
        void IEntityReference.PostInitialize() => PostInitialize();
        void IEntityReference.PreDispose() => PreDispose();
        void IEntityReference.Dispose() => Dispose();
        void IEntityReference.PostDispose() => PostDispose();
    }
}