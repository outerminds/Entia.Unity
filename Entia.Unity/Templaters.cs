using System;
using System.Collections.Generic;
using System.Reflection;
using Entia.Core;
using Entia.Initializers;
using Entia.Instantiators;
using Entia.Modules.Template;
using Entia.Unity;
using UnityEngine;

namespace Entia.Templaters
{
    public sealed class Object : ITemplater
    {
        public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
        {
            if (context.Index == 0 && context.Value is UnityEngine.Object @object)
                return (new UnityInstantiate(@object), new Identity());
            else
                return (new Constant(context.Value), new Identity());
        }
    }

    public sealed class EntityReference : ITemplater
    {
        public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
        {
            throw new NotImplementedException();
        }

        public sealed class Component : Initializer<IComponentReference>
        {
            public readonly int Reference;
            public readonly (FieldInfo field, int reference)[] Fields;
            public readonly Modules.Components Components;

            public Component(int reference, (FieldInfo field, int reference)[] fields, Modules.Components components)
            {
                Reference = reference;
                Fields = fields;
                Components = components;
            }

            public override Result<Unit> Initialize(IComponentReference instance, object[] instances)
            {
                try
                {
                    var entity = (Entity)instances[Reference];
                    var component = instance.Raw;
                    for (int i = 0; i < Fields.Length; i++)
                    {
                        var (field, reference) = Fields[i];
                        field.SetValue(component, instances[reference]);
                    }
                    Components.Set(entity, component);
                    return Result.Success();
                }
                catch (Exception exception) { return Result.Exception(exception); }
            }
        }

        // public Result<(IInstantiator instantiator, IInitializer initializer)> Template(in Context context, World world)
        // {
        //     if (context.Index == 0 && context.Value is Unity.EntityReference reference)
        //     {
        //         Register(reference.gameObject, context);
        //         return (new UnityInstantiate(reference), new Identity());
        //     }
        //     else
        //         return (new Constant(context.Value), new Identity());
        // }

        // void Register(UnityEngine.GameObject @object, in Context context)
        // {
        //     var entity = @object.GetComponent<IEntityReference>();
        //     foreach (var component in @object.GetComponents<UnityEngine.Component>())
        //     {
        //         switch (component)
        //         {
        //             case IEntityReference _: break;
        //             case IComponentReference reference:

        //                 break;
        //             case Transform transform:
        //                 var transformReference = context.Add(transform, new GetTransform(context.Index), new Identity());
        //                 for (int i = 0; i < transform.childCount; i++)
        //                 {
        //                     var child = transform.GetChild(i).gameObject;
        //                     var childReference = context.Add(child, new GetChild(transformReference.Index, i), new Identity());
        //                     Register(child, new Context(childReference.Index, context));
        //                 }
        //                 break;
        //             default:
        //                 context.Add(component, new GetComponent(context.Index, component.GetType()), new Identity());
        //                 break;
        //         }
        //     }
        // }
    }
}