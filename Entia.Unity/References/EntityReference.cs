using Entia.Core;
using Entia.Modules;
using Entia.Components;
using System;
using UnityEngine;
using Entia.Delegates;

namespace Entia.Unity
{
    public interface IEntityReference : IReference
    {
        Entity Entity { get; }

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

        public World World { get; private set; }
        public Entity Entity { get; private set; }
        public string Name => _name ?? (_name = name);

        [NonSerialized]
        States _initialized;
        [NonSerialized]
        States _disposed;
        [NonSerialized]
        string _name;

        void OnEnable() => World?.Components().Remove<IsDisabled>(Entity);
        void OnDisable() => World?.Components().Set<IsDisabled>(Entity, default);

        void Awake()
        {
            if (gameObject.TryWorld(out var world))
            {
                using (var list = PoolUtility.Cache<IEntityReference>.Lists.Use())
                {
                    GetComponentsInChildren(true, list.Instance);
                    foreach (var entity in list.Instance) entity.PreInitialize();
                    foreach (var entity in list.Instance) entity.Initialize(world);
                    foreach (var entity in list.Instance) entity.PostInitialize();
                }
            }
        }

        void OnDestroy()
        {
            // NOTE: no need to dispose the hierarchy since 'OnDestroy' will be called on the children for sure
            this.DisposeAll();
        }

        void OnTransformParentChanged()
        {
            if (World is World world && Entity)
            {
                var families = world.Families();
                var parent = transform.parent;
                if (parent == null)
                {
                    families.Reject(Entity);
                    return;
                }
                var reference = parent.GetComponent<EntityReference>();
                if (reference == null)
                {
                    families.Reject(Entity);
                    return;
                }
                families.Adopt(reference.Entity, Entity);
            }
        }

        void PreInitialize() => _initialized.Change(_initialized | States.Pre);

        void Initialize(World world)
        {
            if (world == null) return;
            if (!Application.isPlaying || _initialized.Change(_initialized | States.Current))
            {
                World = world;
                Entity = World.Entities().Create();
            }
        }

        void PostInitialize()
        {
            if (World == null || Entity == Entity.Zero) return;
            if (!Application.isPlaying || _initialized.Change(_initialized | States.Post))
            {
                var components = World.Components();
                if (UnityEngine.Debug.isDebugBuild) components.Set(Entity, new Components.Debug { Name = Name });
                components.Set(Entity, new Components.Unity<UnityEngine.GameObject> { Value = gameObject });

                using (var list = PoolUtility.Cache<Component>.Lists.Use())
                {
                    GetComponents(list.Instance);
                    foreach (var component in list.Instance)
                    {
                        if (component == null) continue;
                        switch (component)
                        {
                            case IComponentReference reference: reference.Initialize(Entity, World); break;
                            default:
                                if (World.Container.TryGet<IDelegate>(component.GetType(), out var @delegate))
                                    @delegate.Set(component, Entity, World);
                                break;
                        }
                    }
                }

                OnTransformParentChanged();
                if (enabled && gameObject.activeInHierarchy) OnEnable();
                else OnDisable();
            }
        }

        void PreDispose()
        {
            if (World == null || Entity == Entity.Zero) return;
            if (_initialized == States.All && _disposed.Change(_disposed | States.Pre))
            {
                var components = World.Components();
                using (var list = PoolUtility.Cache<Component>.Lists.Use())
                {
                    GetComponents(list.Instance);
                    foreach (var component in list.Instance)
                    {
                        if (component == null) continue;
                        switch (component)
                        {
                            case IComponentReference reference: reference.Dispose(); break;
                            default:
                                if (World.Container.TryGet<IDelegate>(component.GetType(), out var @delegate))
                                    @delegate.Remove(component, Entity, World);
                                break;
                        }
                    }
                }

                components.Remove<Components.Unity<UnityEngine.GameObject>>(Entity);
            }
        }

        void Dispose()
        {
            if (World == null || Entity == Entity.Zero) return;
            if (_initialized == States.All && _disposed.Change(_disposed | States.Current))
            {
                World.Entities().Destroy(Entity);
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
        void IEntityReference.Initialize(World world) => Initialize(world);
        void IEntityReference.PostInitialize() => PostInitialize();
        void IEntityReference.PreDispose() => PreDispose();
        void IEntityReference.Dispose() => Dispose();
        void IEntityReference.PostDispose() => PostDispose();
    }
}