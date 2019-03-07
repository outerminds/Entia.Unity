using Entia.Core;
using Entia.Modules;
using Entia.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Entia.Delegables;

namespace Entia.Unity
{
    public interface IEntityReference
    {
        World World { get; }
        Entity Entity { get; }

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

        static readonly Pool<List<IEntityReference>> _entities = new Pool<List<IEntityReference>>(
            () => new List<IEntityReference>(),
            dispose: instance => instance.Clear());
        static readonly Pool<List<Component>> _components = new Pool<List<Component>>(
            () => new List<Component>(),
            dispose: instance => instance.Clear());

        public World World { get; private set; }
        public Entity Entity { get; private set; }

        States _initialized;
        States _disposed;

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
                    using (var list = _entities.Use())
                    {
                        GetComponentsInChildren(list.Instance);
                        foreach (var entity in list.Instance) entity.Initialize(world, false);
                    }
                }
            }
        }

        void PostInitialize()
        {
            if (World == null || Entity == Entity.Zero) return;
            if (_initialized.Change(_initialized | States.Post))
            {
                var components = World.Components();
                var delegates = World.Delegates();

                if (UnityEngine.Debug.isDebugBuild) components.Set(Entity, new Components.Debug { Name = name });
                if (enabled && gameObject.activeInHierarchy) OnEnable();
                else OnDisable();

                components.Set(Entity, new Components.Unity<UnityEngine.GameObject> { Value = gameObject });

                using (var list = _components.Use())
                {
                    GetComponents(list.Instance);
                    foreach (var component in list.Instance)
                    {
                        switch (component)
                        {
                            case IEntityReference _: break;
                            case IComponentReference reference: reference.Initialize(Entity, World); break;
                            default: delegates.Get(component.GetType()).Set(component, Entity, World); break;
                        }
                    }
                }
            }
        }

        void PreDispose()
        {
            if (_initialized == States.All && _disposed.Change(_disposed | States.Pre))
            {
                var components = World.Components();
                var delegates = World.Delegates();

                using (var list = _components.Use())
                {
                    GetComponents(list.Instance);
                    foreach (var component in list.Instance)
                    {
                        switch (component)
                        {
                            case IEntityReference _: break;
                            case IComponentReference reference: reference.Dispose(); break;
                            default: delegates.Get(component.GetType()).Remove(component, Entity, World); break;
                        }
                    }
                }

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

        void IEntityReference.PreInitialize() => PreInitialize();
        void IEntityReference.Initialize(World world, bool propagate) => Initialize(world, propagate);
        void IEntityReference.PostInitialize() => PostInitialize();
        void IEntityReference.PreDispose() => PreDispose();
        void IEntityReference.Dispose() => Dispose();
        void IEntityReference.PostDispose() => PostDispose();
    }
}