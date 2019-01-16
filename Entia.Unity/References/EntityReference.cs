using Entia.Core;
using Entia.Modules;
using Entia.Unity.Components;
using Entia.Unity.Mappers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entia.Unity
{
    public interface IEntityReference
    {
        World World { get; }
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

        static readonly List<IEntityReference> _entities = new List<IEntityReference>();
        static readonly List<Component> _components = new List<Component>();

        public World World { get; private set; }
        public Entity Entity { get; private set; }

        States _initialized;
        States _disposed;

        void Awake()
        {
            if (WorldRegistry.TryGet(gameObject.scene, out var reference) && reference.World is World world)
            {
                PreInitialize();
                Initialize(world, true);
                PostInitialize();
            }
        }

        void OnEnable() => World?.Components().Remove<IsDisabled>(Entity);
        void OnDisable() => World?.Components().Set(Entity, default(IsDisabled));
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

                if (Application.isEditor) components.Set(Entity, new Components.Debug { Name = gameObject.name });
                components.Set(Entity, new Components.Unity<GameObject> { Value = gameObject });

                GetComponents(_components);
                foreach (var current in _components)
                {
                    switch (current)
                    {
                        case IComponentReference component: component.Initialize(Entity, World); break;
                        default: current.Map<SetComponent, Unit>(new SetComponent(Entity, components)); break;
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
                foreach (var current in _components)
                {
                    switch (current)
                    {
                        case IComponentReference component: component.Dispose(); break;
                        default: current.Map<RemoveComponent, Unit>(new RemoveComponent(Entity, components)); break;
                    }
                }
                _components.Clear();

                components.Remove<Components.Unity<GameObject>>(Entity);
                if (Application.isEditor) components.Remove<Components.Debug>(Entity);
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
            if (_initialized == States.All) _disposed.Change(_disposed | States.Post);
        }

        void IEntityReference.PreInitialize() => PreInitialize();
        void IEntityReference.Initialize(World world) => Initialize(world, false);
        void IEntityReference.PostInitialize() => PostInitialize();
        void IEntityReference.PreDispose() => PreDispose();
        void IEntityReference.Dispose() => Dispose();
        void IEntityReference.PostDispose() => PostDispose();
    }
}