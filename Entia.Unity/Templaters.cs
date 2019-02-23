using Entia.Core;
using Entia.Initializers;
using Entia.Instantiators;
using Entia.Modules;
using Entia.Modules.Template;
using Entia.Templaters;
using Entia.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Entia.Templaters
{
    public sealed class GameObject : Templater<UnityEngine.GameObject>
    {
        public override Result<IInitializer> Initializer(UnityEngine.GameObject value, Context context, World world)
        {
            var result = Result.Left(
                Enumerable.Range(0, value.transform.childCount)
                    .Select(index => world­.Templaters().Template(value.transform.GetChild(index).gameObject, context).Map(element => element.Reference))
                    .All(),
                value.GetComponents<UnityEngine.Component>()
                    .Select(component => world.Templaters().Template(component, context).Map(element => element.Reference))
                    .All());
            if (result.TryFailure(out var failure)) return failure;
            if (result.TryValue(out var children)) return new Initializers.GameObject(value.name, value.tag, value.layer, value.activeSelf, children);
            return Result.Failure();
        }

        public override Result<IInstantiator> Instantiator(UnityEngine.GameObject value, Context context, World world) => new Instantiators.GameObject();
    }

    public abstract class Component<T> : Templater<T> where T : UnityEngine.Component
    {
        public override Result<IInstantiator> Instantiator(T value, Context context, World world)
        {
            if (context.Indices.TryGetValue(new Context.Key(value.gameObject), out var index))
                return new Instantiators.Component(value.GetType(), index);
            return new Constant(value);
        }
    }

    public sealed class Component : Component<UnityEngine.Component>
    {
        public override Result<IInitializer> Initializer(UnityEngine.Component value, Context context, World world)
        {
            try
            {
                var properties = value.GetType().Bases().Prepend(value.GetType())
                    .TakeWhile(type => type.IsSubclassOf(typeof(UnityEngine.Component)))
                    .SelectMany(type => type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
                    .Where(property => property.CanRead && property.CanWrite)
                    .Distinct()
                    .ToArray();
                var members = new List<(PropertyInfo, int)>(properties.Length);
                for (var i = 0; i < properties.Length; i++)
                {
                    var property = properties[i];
                    var member = property.GetValue(value);

                    var result = world.Templaters().Template(member, context);
                    if (result.TryFailure(out var failure)) return failure;
                    if (result.TryValue(out var element)) members.Add((property, element.Reference));
                }

                return new Entia.Initializers.Object(properties: members.ToArray());
            }
            catch (Exception exception) { return Result.Exception(exception); }
        }
    }

    public sealed class Transform : Templater<UnityEngine.Transform>
    {
        public override Result<IInitializer> Initializer(UnityEngine.Transform value, Context context, World world)
        {
            var position = value.localPosition;
            var rotation = value.localRotation;
            var scale = value.localScale;
            return new Function<UnityEngine.Transform>(instance =>
            {
                instance.localPosition = position;
                instance.localRotation = rotation;
                instance.localScale = scale;
            });
        }

        public override Result<IInstantiator> Instantiator(UnityEngine.Transform value, Context context, World world)
        {
            if (context.Indices.TryGetValue(new Context.Key(value.gameObject), out var index)) return new Instantiators.Transform(index);
            return new Constant(value);
        }
    }
}
