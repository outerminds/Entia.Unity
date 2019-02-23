using Entia.Core;
using Entia.Modules;
using Entia.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using Entia.Initializers;
using Entia.Templaters;
using Entia.Modules.Template;
using System.Linq;
using Entia.Instantiators;

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

        sealed class Initializer : Initializer<Entia.Entity>
        {
            public readonly int[] Components;
            public readonly World World;

            public Initializer(int[] components, World world)
            {
                Components = components;
                World = world;
            }

            public override Result<Unit> Initialize(Entia.Entity instance, object[] instances)
            {
                var components = World.Components();
                components.Clear(instance);

                foreach (var component in Components)
                {
                    var result = Result.Cast<IComponent>(instances[component]);
                    if (result.TryValue(out var value)) components.Set(instance, value);
                    else return result;
                }

                return Result.Success();
            }
        }

        sealed class Templater : Templater<EntityReference>
        {
            public override Result<IInitializer> Initializer(Unity.EntityReference value, Context context, World world)
            {
                var result = value.gameObject.GetComponents<UnityEngine.Component>()
                    .OfType<IComponentReference>()
                    .Select(reference => reference.Value)
                    .Append(new Components.Debug { Name = value.name })
                    .Select(component => world.Templaters().Template(component, context).Map(element => element.Reference))
                    .All();
                if (result.TryFailure(out var failure)) return failure;
                if (result.TryValue(out var components)) return new Initializers.EntityReference(components, world);
                return Result.Failure();
            }

            public override Result<IInstantiator> Instantiator(Unity.EntityReference value, Context context, World world) =>
                new Instantiators.Factory<Entity>(() => world.Entities().Create());
        }

        [Templater]
        static readonly Templater _templater = new Templater();
        static readonly List<IEntityReference> _entities = new List<IEntityReference>();
        static readonly List<UnityEngine.Component> _components = new List<UnityEngine.Component>();

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

                if (UnityEngine.Debug.isDebugBuild) components.Set(Entity, new Components.Debug { Name = gameObject.name });
                if (gameObject.activeInHierarchy) OnEnable();
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