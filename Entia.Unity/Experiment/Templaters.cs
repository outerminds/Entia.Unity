// // using Entia.Core;
// // using Entia.Initializers;
// // using Entia.Instantiators;
// // using Entia.Modules.Template;

// // namespace Entia.Templaters
// // {
// //     public sealed class Object : ITemplater
// //     {
// //         public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
// //         {
// //             if (context.Index == 0 && context.Value is UnityEngine.Object @object)
// //                 return (new UnityInstantiate(@object), new Identity());
// //             else
// //                 return (new Constant(context.Value), new Identity());
// //         }
// //     }
// // }


// sealed class Templater : ITemplater
// {
//     sealed class PoolInstantiate : Instantiator<Instance>
//     {
//         public readonly Pool Pool;
//         public PoolInstantiate(Pool pool) { Pool = pool; }
//         public override Result<Instance> Instantiate(object[] instances) => Pool.Take();
//     }

//     sealed class PoolInitialize : Initializer<Instance>
//     {
//         public readonly int Reference;
//         public readonly Pool Pool;
//         public readonly World World;

//         public PoolInitialize(int reference, Pool pool, World world)
//         {
//             Reference = reference;
//             Pool = pool;
//             World = world;
//         }

//         public override Result<Unit> Initialize(Instance instance, object[] instances)
//         {
//             var result = Result.Cast<Entity>(instances[Reference]);
//             if (result.TryValue(out var entity))
//             {
//                 var components = World.Components();
//                 if (UnityEngine.Debug.isDebugBuild) components.Set(entity, new Components.Debug { Name = instance.Name });
//                 components.Set(entity, new Components.Unity<GameObject> { Value = instance.GameObject });
//                 foreach (var component in instance.Components) components.Set(entity, component);
//                 foreach (var component in Pool.References) components.Set(entity, component.Value);
//                 return Result.Success();
//             }
//             return result;
//         }
//     }

//     sealed class Pool
//     {
//         public Transform Root;
//         public GameObject Template;
//         public GameObject Stripped;
//         public IComponentReference[] References;
//         public World World;

//         readonly Stack<Instance> _instances = new Stack<Instance>();

//         public Instance Take()
//         {
//             if (_instances.Count > 0)
//             {
//                 var instance = _instances.Pop();
//                 instance.GameObject.SetActive(true);
//                 return instance;
//             }
//             else
//             {
//                 var delegates = World.Delegates();
//                 var name = Template.name;
//                 var instance = UnityEngine.Object.Instantiate(Stripped, Root);
//                 instance.name = name;
//                 instance.SetActive(true);

//                 using (var list = PoolUtility.Cache<Component>.Lists.Use())
//                 {
//                     instance.GetComponents(list.Instance);
//                     return new Instance
//                     {
//                         Name = name,
//                         GameObject = instance,
//                         Transform = instance.transform,
//                         Components = list.Instance
//                             .TrySelect((Component unity, out IComponent component) => delegates.Get(unity.GetType()).TryCreate(unity, out component))
//                             .ToArray(),
//                         Children = new Instance[0]
//                     };
//                 }
//             }
//         }

//         public void Put(Instance instance)
//         {
//             instance.GameObject.SetActive(false);
//             instance.Transform.parent = Root;
//             _instances.Push(instance);
//         }
//     }

//     sealed class Instance
//     {
//         public string Name;
//         public GameObject GameObject;
//         public Transform Transform;
//         public IComponent[] Components;
//         public Instance[] Children;
//     }

//     readonly Dictionary<GameObject, Pool> _pools = new Dictionary<GameObject, Pool>();

//     Pool GetPool(GameObject template, World world)
//     {
//         if (_pools.TryGetValue(template, out var pool)) return pool;

//         var active = template.activeSelf;
//         template.SetActive(false);
//         var root = new GameObject(template.name).transform;
//         var stripped = UnityEngine.Object.Instantiate(template, root);
//         stripped.name = "Template";
//         template.SetActive(active);

//         using (var list = PoolUtility.Cache<Component>.Lists.Use())
//         {
//             stripped.GetComponents(list.Instance);
//             foreach (var unity in list.Instance) if (unity is IComponentReference) UnityEngine.Object.DestroyImmediate(unity);
//             foreach (var unity in list.Instance) if (unity is IEntityReference) UnityEngine.Object.DestroyImmediate(unity);
//         }

//         return _pools[template] = new Pool
//         {
//             Root = root,
//             Template = template,
//             Stripped = stripped,
//             References = template.GetComponents<IComponentReference>(),
//             World = world,
//         };
//     }

//     Instance GetInstance(GameObject template, World world) => GetPool(template, world).Take();

//     public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
//     {
//         if (context.Index == 0 && context.Value is Unity.EntityReference root)
//         {
//             var pool = GetPool(root.gameObject, world);
//             var pooled = context.Add(root.gameObject, new PoolInstantiate(pool), new PoolInitialize(context.Index, pool, world));
//             return (new Entity.Instantiator(world.Entities()), new Identity());
//         }
//         else
//             return (new Constant(context.Value), new Identity());
//     }
// }
