// using System;
// using Entia.Core;
// using UnityEngine;

// namespace Entia.Instantiators
// {
//     public sealed class UnityInstantiate : IInstantiator
//     {
//         public readonly UnityEngine.Object Object;
//         public UnityInstantiate(UnityEngine.Object @object) { Object = @object; }
//         public Result<object> Instantiate(object[] instances) => UnityEngine.Object.Instantiate(Object);
//     }

//     public sealed class GetChild : IInstantiator
//     {
//         public readonly int Reference;
//         public readonly int Index;

//         public GetChild(int reference, int index)
//         {
//             Reference = reference;
//             Index = index;
//         }

//         public Result<object> Instantiate(object[] instances)
//         {
//             switch (instances[Reference])
//             {
//                 case Transform transform: return transform.GetChild(Index).gameObject;
//                 case GameObject gameObject: return gameObject.transform.GetChild(Index).gameObject;
//                 case Component component: return component.transform.GetChild(Index).gameObject;
//                 default: return null;
//             }
//         }
//     }

//     public sealed class GetGameObject : IInstantiator
//     {
//         public readonly int Reference;

//         public GetGameObject(int reference) { Reference = reference; }

//         public Result<object> Instantiate(object[] instances)
//         {
//             switch (instances[Reference])
//             {
//                 case GameObject gameObject: return gameObject;
//                 case Component component: return component.gameObject;
//                 default: return null;
//             }
//         }
//     }

//     public sealed class GetTransform : IInstantiator
//     {
//         public readonly int Reference;

//         public GetTransform(int reference) { Reference = reference; }

//         public Result<object> Instantiate(object[] instances)
//         {
//             switch (instances[Reference])
//             {
//                 case Transform transform: return transform;
//                 case GameObject gameObject: return gameObject.transform;
//                 case Component component: return component.transform;
//                 default: return null;
//             }
//         }
//     }

//     public sealed class GetComponent : IInstantiator
//     {
//         public readonly int Reference;
//         public readonly Type Type;

//         public GetComponent(int reference, Type type)
//         {
//             Reference = reference;
//             Type = type;
//         }

//         public Result<object> Instantiate(object[] instances)
//         {
//             switch (instances[Reference])
//             {
//                 case GameObject gameObject: return gameObject.GetComponent(Type);
//                 case Component component: return component.GetComponent(Type);
//                 default: return null;
//             }
//         }
//     }
// }
