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
            sealed class PoolInstantiate : Instantiator<Instance>
            {
                public readonly Pool Pool;
                public PoolInstantiate(Pool pool) { Pool = pool; }
                public override Result<Instance> Instantiate(object[] instances) => Pool.Take();
            }

            sealed class PoolInitialize : Initializer<Instance>
            {
                public readonly int Reference;
                public readonly Pool Pool;
                public readonly World World;

                public PoolInitialize(int reference, Pool pool, World world)
                {
                    Reference = reference;
                    Pool = pool;
                    World = world;
                }

                public override Result<Unit> Initialize(Instance instance, object[] instances)
                {
                    var result = Result.Cast<Entity>(instances[Reference]);
                    if (result.TryValue(out var entity))
                    {
                        var components = World.Components();
                        if (UnityEngine.Debug.isDebugBuild) components.Set(entity, new Components.Debug { Name = instance.Name });
                        components.Set(entity, new Components.Unity<GameObject> { Value = instance.GameObject });
                        foreach (var component in instance.Components) components.Set(entity, component);
                        foreach (var component in Pool.References) components.Set(entity, component.Value);
                        return Result.Success();
                    }
                    return result;
                }
            }

            sealed class Pool
            {
                public Transform Root;
                public GameObject Template;
                public GameObject Stripped;
                public IComponentReference[] References;
                public World World;

                readonly Stack<Instance> _instances = new Stack<Instance>();

                public Instance Take()
                {
                    if (_instances.Count > 0)
                    {
                        var instance = _instances.Pop();
                        instance.GameObject.SetActive(true);
                        return instance;
                    }
                    else
                    {
                        var delegates = World.Delegates();
                        var name = Template.name;
                        var instance = UnityEngine.Object.Instantiate(Stripped, Root);
                        instance.name = name;
                        instance.SetActive(true);

                        using (var list = _components.Use())
                        {
                            instance.GetComponents(list.Instance);
                            return new Instance
                            {
                                Name = name,
                                GameObject = instance,
                                Transform = instance.transform,
                                Components = list.Instance
                                    .TrySelect((Component unity, out IComponent component) => delegates.Get(unity.GetType()).TryCreate(unity, out component))
                                    .ToArray(),
                                Children = new Instance[0]
                            };
                        }
                    }
                }

                public void Put(Instance instance)
                {
                    instance.GameObject.SetActive(false);
                    instance.Transform.parent = Root;
                    _instances.Push(instance);
                }
            }

            sealed class Instance
            {
                public string Name;
                public GameObject GameObject;
                public Transform Transform;
                public IComponent[] Components;
                public Instance[] Children;
            }

            readonly Dictionary<GameObject, Pool> _pools = new Dictionary<GameObject, Pool>();

            Pool GetPool(GameObject template, World world)
            {
                if (_pools.TryGetValue(template, out var pool)) return pool;

                var active = template.activeSelf;
                template.SetActive(false);
                var root = new GameObject(template.name).transform;
                var stripped = UnityEngine.Object.Instantiate(template, root);
                stripped.name = "Template";
                template.SetActive(active);

                using (var list = _components.Use())
                {
                    stripped.GetComponents(list.Instance);
                    foreach (var unity in list.Instance) if (unity is IComponentReference) UnityEngine.Object.DestroyImmediate(unity);
                    foreach (var unity in list.Instance) if (unity is IEntityReference) UnityEngine.Object.DestroyImmediate(unity);
                }

                return _pools[template] = new Pool
                {
                    Root = root,
                    Template = template,
                    Stripped = stripped,
                    References = template.GetComponents<IComponentReference>(),
                    World = world,
                };
            }

            Instance GetInstance(GameObject template, World world) => GetPool(template, world).Take();

            public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
            {
                if (context.Index == 0 && context.Value is Unity.EntityReference root)
                {
                    var pool = GetPool(root.gameObject, world);
                    var pooled = context.Add(root.gameObject, new PoolInstantiate(pool), new PoolInitialize(context.Index, pool, world));
                    return (new Entity.Instantiator(world.Entities()), new Identity());
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