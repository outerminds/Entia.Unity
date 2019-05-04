using Entia.Core;
using Entia.Modules;
using Entia.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Entia.Delegables;
using Entia.Templaters;
using Entia.Instantiators;
using Entia.Delegates;
using Entia.Initializers;
using Entia.Modules.Template;
using Entia.Templateables;

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
    public sealed class EntityReference : MonoBehaviour, IEntityReference, ITemplateable<EntityReference.Templater>
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

        sealed class Templater : ITemplater
        {
            sealed class PoolInstantiate : IInstantiator
            {
                public readonly GameObject Template;

                public PoolInstantiate(GameObject template) { Template = template; }

                public Result<object> Instantiate(object[] instances)
                {
                    // TODO: get the template instance from a pool instead
                    var instance = UnityEngine.Object.Instantiate(Template);
                    instance.SetActive(true);
                    return instance;
                }
            }

            sealed class UnityGameObject : Instantiator<Components.Unity<GameObject>>
            {
                public readonly int Reference;
                public UnityGameObject(int reference) { Reference = reference; }
                public override Result<Components.Unity<GameObject>> Instantiate(object[] instances) =>
                    new Components.Unity<GameObject> { Value = instances[Reference] as GameObject };
            }

            sealed class UnityComponent : Instantiator<IComponent>
            {
                public readonly int Reference;
                public readonly Type Type;
                public readonly IDelegate Delegate;

                public UnityComponent(int reference, Type type, IDelegate @delegate)
                {
                    Reference = reference;
                    Type = type;
                    Delegate = @delegate;
                }

                public override Result<IComponent> Instantiate(object[] instances)
                {
                    var gameObject = instances[Reference] as GameObject;
                    var component = gameObject.GetComponent(Type);
                    Delegate.TryCreate(component, out var unity);
                    return Result.Cast<IComponent>(unity);
                }
            }

            public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
            {
                // NOTE: the 'UnityComponent' instantiation can be cached in an 'Instance' data structure managed by the pool;
                // i.e. the pool will cache an instance of 'Unity<T>' ready to be added to the entity

                if (context.Index == 0 && context.Value is Unity.EntityReference root)
                {
                    var delegates = world.Delegates();
                    var templaters = world.Templaters();
                    var active = root.gameObject.activeSelf;
                    root.gameObject.SetActive(false);
                    var stripped = UnityEngine.Object.Instantiate(root.gameObject);
                    root.gameObject.SetActive(active);
                    var pooled = context.Add(root.gameObject, new PoolInstantiate(stripped), new Identity());
                    var indices = new List<int>();

                    {
                        var wrapped = new Components.Unity<GameObject> { Value = stripped };
                        var reference = context.Add(wrapped, new UnityGameObject(pooled.Index), new Identity());
                        indices.Add(reference.Index);
                    }

                    using (var list = _components.Use())
                    {
                        stripped.GetComponents(list.Instance);
                        foreach (var unity in list.Instance)
                        {
                            switch (unity)
                            {
                                case IEntityReference entity: break;
                                case IComponentReference component:
                                    UnityEngine.Object.DestroyImmediate(unity);
                                    break;
                                default:
                                    var type = unity.GetType();
                                    var @delegate = delegates.Get(type);
                                    @delegate.TryCreate(unity, out var wrapped);
                                    var reference = context.Add(wrapped, new UnityComponent(pooled.Index, type, @delegate), new Identity());
                                    indices.Add(reference.Index);
                                    break;
                            }
                        }
                    }

                    using (var list = _entities.Use())
                    {
                        stripped.GetComponents(list.Instance);
                        foreach (var entity in list.Instance) UnityEngine.Object.DestroyImmediate(entity as UnityEngine.Object);
                    }

                    return (new Entity.Instantiator(world.Entities()), new Entity.Initializer(indices.ToArray(), world));
                }
                else
                    return (new Constant(context.Value), new Identity());
            }
        }

        static readonly Pool<List<IEntityReference>> _entities = new Pool<List<IEntityReference>>(
            () => new List<IEntityReference>(),
            dispose: instance => instance.Clear());
        static readonly Pool<List<Component>> _components = new Pool<List<Component>>(
            () => new List<Component>(),
            dispose: instance => instance.Clear());

        public World World { get; private set; }
        public Entity Entity { get; private set; }
        public string Name => _name ?? (_name = name);

        States _initialized;
        States _disposed;
        string _name;

        void OnEnable() => World?.Components().Remove<IsDisabled>(Entity);
        void OnDisable() => World?.Components().Set<IsDisabled>(Entity, default);

        void Awake()
        {
            if (gameObject.TryWorld(out var world))
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
                World.Components().Set(Entity, new Components.Unity<EntityReference> { Value = this });

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

                if (UnityEngine.Debug.isDebugBuild) components.Set(Entity, new Components.Debug { Name = Name });
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
            }
        }

        void Dispose()
        {
            if (World == null || Entity == Entity.Zero) return;
            if (_initialized == States.All && _disposed.Change(_disposed | States.Current))
            {
                World.Components().Remove<Components.Unity<EntityReference>>(Entity);
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
        void IEntityReference.Initialize(World world, bool propagate) => Initialize(world, propagate);
        void IEntityReference.PostInitialize() => PostInitialize();
        void IEntityReference.PreDispose() => PreDispose();
        void IEntityReference.Dispose() => Dispose();
        void IEntityReference.PostDispose() => PostDispose();
    }
}