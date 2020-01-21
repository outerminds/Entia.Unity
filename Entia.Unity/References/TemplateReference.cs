using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entia.Core;
using Entia.Experimental.Instantiators;
using Entia.Experimental.Serialization;
using Entia.Experimental.Serializers;
using Entia.Experimental.Templaters;
using Entia.Experimental.Templating;
using Entia.Experimental.Values;
using Entia.Modules;
using UnityEngine;

namespace Entia.Experimental
{
    public sealed class TemplateReference : MonoBehaviour, ISerializationCallbackReceiver
    {
        static readonly object[] _types =
        {
            typeof(Values.Primitive), typeof(Values.Array), typeof(Values.Object),
            typeof(Values.Primitive<bool>), typeof(Values.Array<bool>),
            typeof(Values.Primitive<char>), typeof(Values.Array<char>),
            typeof(Values.Primitive<byte>), typeof(Values.Array<byte>),
            typeof(Values.Primitive<sbyte>), typeof(Values.Array<sbyte>),
            typeof(Values.Primitive<ushort>), typeof(Values.Array<ushort>),
            typeof(Values.Primitive<short>), typeof(Values.Array<short>),
            typeof(Values.Primitive<uint>), typeof(Values.Array<uint>),
            typeof(Values.Primitive<int>), typeof(Values.Array<int>),
            typeof(Values.Primitive<ulong>), typeof(Values.Array<ulong>),
            typeof(Values.Primitive<long>), typeof(Values.Array<long>),
            typeof(Values.Primitive<float>), typeof(Values.Array<float>),
            typeof(Values.Primitive<double>), typeof(Values.Array<double>),
            typeof(Values.Primitive<decimal>), typeof(Values.Array<decimal>),
            typeof(Values.Primitive<string>), typeof(Values.Array<string>),
            typeof(Values.Primitive<object>), typeof(Values.Array<object>),
            // NOTE: for some reason, Unity crashes when these generic definitions are placed before their concrete instances
            typeof(Values.Primitive<>), typeof(Values.Array<>), typeof(Values.Object<>),
            typeof(Values.Entity),
            typeof(Values.Unity),

            typeof(Components.Debug),
            typeof(Components.Unity<Transform>),
            typeof(Components.Unity<GameObject>),
            typeof(Components.Unity<>),
        };

        static Disposable Extract(IValue[] values, out UnityEngine.Object[] references, out int[] indices)
        {
            IEnumerable<(UnityEngine.Object, int)> Collect()
            {
                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] is Values.Unity unity)
                    {
                        yield return (unity.Value, i);
                        values[i] = default;
                    }
                }
            }

            var pair = Collect().ToArray().Unzip();
            (references, indices) = pair;
            return new Disposable(() => Restore(values, pair.Item1, pair.Item2));
        }

        static void Restore(IValue[] values, UnityEngine.Object[] references, int[] indices)
        {
            for (int i = 0; i < references.Length; i++)
                values[indices[i]] = new Values.Unity(references[i]);
        }

        public Template<Entity> Template = Template<Entity>.Empty;

        [SerializeField] UnityEngine.Object[] _references = { };
        [SerializeField] int[] _indices = { };
        [SerializeField] byte[] _bytes = { };

        public void OnBeforeSerialize()
        {
            var world = new World();
            using (Extract(Template.Values, out _references, out _indices))
                world.Serialize(Template, out _bytes, Options.None, _types);
        }

        public void OnAfterDeserialize()
        {
            var world = new World();
            world.Deserialize(_bytes, out Template, Options.None, _types);
            Restore(Template.Values, _references, _indices);
        }
    }
}

namespace Entia.Experimental.Values
{
    public readonly struct Member
    {
        public readonly string Name;
        public readonly int Reference;

        public Member(string name, int reference)
        {
            Name = name;
            Reference = reference;
        }
    }

    public interface IValue
    {
        object Value { get; }
        Member[] Members { get; }
    }

    /*
        - includes primitive types, strings, types, assemblies and some other types
        - includes structs that have no fields
        - includes structs that implement no interface or only empty interfaces or interfaces with only getters
            - while it's true that a boxed struct may mutate its fields through the 'System.Object' methods,
            this would clearly be looking for trouble, and so it is assumed that this is not the case
            - same goes with getters
        - warning: if the struct is being modified by reflection, there may be a problem...
            - could be simpler and safer to just clone the struct
    */
    public readonly struct Primitive : IValue, IImplementation<Primitive.Instantiator>
    {
        sealed class Instantiator : Instantiator<Primitive>
        {
            public override bool Instantiate(in Primitive value, in Instantiators.Context context, out object instance)
            {
                instance = value.Value;
                return true;
            }
        }

        [Implementation]
        static Serializer<Primitive> _serializer => Serializer.Map(
            (in Primitive primitive) => primitive.Value,
            (in object value) => new Primitive(value));

        public Member[] Members => Dummy<Member>.Read.Array.Zero;
        object IValue.Value => Value;

        public readonly object Value;
        public Primitive(object value) { Value = value; }
    }

    public readonly struct Primitive<T> : IValue, IImplementation<Primitive<T>.Instantiator>
    {
        sealed class Instantiator : Instantiator<Primitive<T>>
        {
            public override bool Instantiate(in Primitive<T> value, in Instantiators.Context context, out object instance)
            {
                instance = value.Value;
                return true;
            }
        }

        [Implementation]
        static Serializer<Primitive<T>> _serializer => Serializer.Map(
            (in Primitive<T> primitive) => primitive.Value,
            (in T value) => new Primitive<T>(value));

        public Member[] Members => Dummy<Member>.Read.Array.Zero;
        object IValue.Value => Value;

        public readonly T Value;
        public Primitive(T value) { Value = value; }
    }

    public readonly struct Object : IValue, IImplementation<Object.Instantiator>
    {
        sealed class Instantiator : Instantiator<Object>
        {
            public override bool Instantiate(in Object value, in Instantiators.Context context, out object instance)
            {
                instance = CloneUtility.Shallow(value.Value);
                foreach (var (field, reference) in value.Fields)
                    field.SetValue(instance, context.Instantiate(reference));
                foreach (var (property, reference) in value.Properties)
                    property.SetValue(instance, context.Instantiate(reference));
                return true;
            }
        }

        [Implementation]
        static Serializer<Object> _serializer => Serializer.Map(
            (in Object @object) =>
            {
                var fields = @object.Fields.Unzip();
                var properties = @object.Properties.Unzip();
                return (@object.Value, fields.Item1, fields.Item2, properties.Item1, properties.Item2);
            },
            (in (object value, FieldInfo[], int[], PropertyInfo[], int[]) values) =>
                new Object(values.value, values.Item2.Zip(values.Item3), values.Item4.Zip(values.Item5)),
            Serializer.Tuple(
                default(Serializer<object>),
                Serializer.Array<FieldInfo>(),
                Serializer.Blittable.Array<int>(),
                Serializer.Array<PropertyInfo>(),
                Serializer.Blittable.Array<int>()));

        public Member[] Members => Enumerable.Concat(
            Fields.Select(field => new Member(field.field.Name, field.reference)),
            Properties.Select(property => new Member(property.property.Name, property.reference))
        ).ToArray();
        object IValue.Value => Value;

        public readonly object Value;
        public readonly (FieldInfo field, int reference)[] Fields;
        public readonly (PropertyInfo property, int reference)[] Properties;

        public Object(object value, (FieldInfo field, int reference)[] fields, (PropertyInfo property, int reference)[] properties)
        {
            Value = value;
            Fields = fields;
            Properties = properties;
        }
    }

    public readonly struct Object<T> : IValue, IImplementation<Object<T>.Instantiator>
    {
        sealed class Instantiator : Instantiator<Object<T>>
        {
            public override bool Instantiate(in Object<T> value, in Instantiators.Context context, out object instance)
            {
                instance = CloneUtility.Shallow(value.Value);
                foreach (var (field, reference) in value.Fields)
                    field.SetValue(instance, context.Instantiate(reference));
                foreach (var (property, reference) in value.Properties)
                    property.SetValue(instance, context.Instantiate(reference));
                return true;
            }
        }

        [Implementation]
        static Serializer<Object<T>> _serializer => Serializer.Map(
            (in Object<T> @object) =>
            {
                var fields = @object.Fields.Unzip();
                var properties = @object.Properties.Unzip();
                return (@object.Value, fields.Item1, fields.Item2, properties.Item1, properties.Item2);
            },
            (in (T value, FieldInfo[], int[], PropertyInfo[], int[]) values) =>
                new Object<T>(values.value, values.Item2.Zip(values.Item3), values.Item4.Zip(values.Item5)),
            Serializer.Tuple(
                default(Serializer<T>),
                Serializer.Array<FieldInfo>(),
                Serializer.Blittable.Array<int>(),
                Serializer.Array<PropertyInfo>(),
                Serializer.Blittable.Array<int>()));

        public Member[] Members => Enumerable.Concat(
            Fields.Select(field => new Member(field.field.Name, field.reference)),
            Properties.Select(property => new Member(property.property.Name, property.reference))
        ).ToArray();
        object IValue.Value => Value;

        public readonly T Value;
        public readonly (FieldInfo field, int reference)[] Fields;
        public readonly (PropertyInfo property, int reference)[] Properties;

        public Object(T value, (FieldInfo field, int reference)[] fields, (PropertyInfo property, int reference)[] properties)
        {
            Value = value;
            Fields = fields;
            Properties = properties;
        }
    }

    public readonly struct Array : IValue, IImplementation<Array.Instantiator>
    {
        sealed class Instantiator : Instantiator<Array>
        {
            public override bool Instantiate(in Array value, in Instantiators.Context context, out object instance)
            {
                var array = CloneUtility.Shallow(value.Value);
                instance = array;
                foreach (var (index, reference) in value.Items)
                    array.SetValue(context.Instantiate(reference), index);
                return true;
            }
        }

        [Implementation]
        static Serializer<Array> _serializer => Serializer.Map(
            (in Array array) => (array.Value, array.Items.Unzip()).Flatten(),
            (in (System.Array value, int[] indices, int[] references) values) => new Array(values.value, values.indices.Zip(values.references)),
            Serializer.Tuple(default(Serializer<System.Array>), Serializer.Blittable.Array<int>(), Serializer.Blittable.Array<int>()));

        public Member[] Members => Items.Select(item => new Member($"{item.index}", item.reference));
        object IValue.Value => Value;

        public readonly System.Array Value;
        public readonly (int index, int reference)[] Items;

        public Array(System.Array value, params (int index, int reference)[] items)
        {
            Value = value;
            Items = items;
        }
    }

    public readonly struct Array<T> : IValue, IImplementation<Array<T>.Instantiator>
    {
        sealed class Instantiator : Instantiator<Array<T>>
        {
            public override bool Instantiate(in Array<T> value, in Instantiators.Context context, out object instance)
            {
                var array = CloneUtility.Shallow(value.Value);
                instance = array;
                foreach (var (index, reference) in value.Items)
                    array[index] = (T)context.Instantiate(reference);
                return true;
            }
        }

        [Implementation]
        static Serializer<Array<T>> _serializer => Serializer.Map(
            (in Array<T> array) => (array.Value, array.Items.Unzip()).Flatten(),
            (in (T[] value, int[] indices, int[] references) values) => new Array<T>(values.value, values.indices.Zip(values.references)),
            Serializer.Tuple(default(Serializer<T[]>), Serializer.Blittable.Array<int>(), Serializer.Blittable.Array<int>()));

        public Member[] Members => Items.Select(item => new Member($"{item.index}", item.reference));
        object IValue.Value => Value;

        public readonly T[] Value;
        public readonly (int index, int reference)[] Items;

        public Array(T[] value, params (int index, int reference)[] items)
        {
            Value = value;
            Items = items;
        }
    }

    public readonly struct Entity : IValue, IImplementation<Entity.Instantiator>
    {
        sealed class Instantiator : Instantiator<Entity>
        {
            public override bool Instantiate(in Entity value, in Instantiators.Context context, out object instance)
            {
                var entity = context.World.Entities().Create();
                instance = entity;

                for (int i = 0; i < value.Components.Length; i++)
                {
                    if (context.Instantiate(value.Components[i]) is IComponent component)
                        context.World.Components().Set(entity, component);
                    else
                        return false;
                }

                for (int i = 0; i < value.Children.Length; i++)
                {
                    if (context.Instantiate(value.Children[i]) is Entia.Entity child)
                        context.World.Families().Adopt(entity, child);
                    else
                        return false;
                }

                return true;
            }
        }

        [Implementation]
        static Serializer<Entity> _serializer => Serializer.Map(
            (in Entity entity) => (entity.Children, entity.Components),
            (in (int[] children, int[] components) values) => new Entity(values.children, values.components),
            Serializer.Tuple(Serializer.Blittable.Array<int>(), Serializer.Blittable.Array<int>()));

        public Member[] Members => Enumerable.Concat(
            Children.Select(child => new Member($"{child}", child)),
            Components.Select(component => new Member($"{component}", component))
        ).ToArray();
        object IValue.Value => Entia.Entity.Zero;

        public readonly int[] Children;
        public readonly int[] Components;

        public Entity(int[] children, int[] components)
        {
            Children = children;
            Components = components;
        }
    }

    public readonly struct Unity : IValue, IImplementation<Unity.Instantiator>
    {
        sealed class Instantiator : Instantiator<Unity>
        {
            public override bool Instantiate(in Unity value, in Instantiators.Context context, out object instance)
            {
                instance = value.Value;
                return true;
            }
        }

        public Member[] Members => Dummy<Member>.Read.Array.Zero;
        object IValue.Value => Value;

        public readonly UnityEngine.Object Value;
        public Unity(UnityEngine.Object value) { Value = value; }
    }
}

namespace Entia.Experimental.Instantiators
{
    public readonly struct Context
    {
        public readonly IValue Value;
        public readonly IValue[] Values;
        public readonly object[] Instances;
        public readonly HashSet<int> Indices;
        public readonly World World;

        public Context(IValue[] values, object[] instances, World world) : this(null, values, instances, new HashSet<int>(), world) { }
        Context(IValue value, IValue[] values, object[] instances, HashSet<int> indices, World world)
        {
            Value = value;
            Values = values;
            Instances = instances;
            Indices = indices;
            World = world;
        }

        public object Instantiate(int index)
        {
            ref var instance = ref Instances[index];
            if (Indices.Add(index))
            {
                var value = Values[index];
                if (value is null)
                    instance = null;
                else if (World.Container.TryGet<IInstantiator>(value.GetType(), out var instantiator))
                    instantiator.Instantiate(With(value), out instance);
                else
                    instance = value;
            }

            return instance;
        }

        public Context With(IValue value) => new Context(value, Values, Instances, Indices, World);
    }

    public interface IInstantiator : ITrait
    {
        bool Instantiate(in Context context, out object instance);
    }

    public abstract class Instantiator<T> : IInstantiator where T : IValue
    {
        public abstract bool Instantiate(in T value, in Context context, out object instance);
        bool IInstantiator.Instantiate(in Context context, out object instance)
        {
            if (context.Value is T value) return Instantiate(value, context, out instance);
            instance = default;
            return false;
        }
    }
}

namespace Entia.Experimental.Templaters
{
    public readonly struct Context
    {
        public readonly object Value;
        public readonly List<IValue> Values;
        public readonly Dictionary<object, int> Indices;
        public readonly World World;

        public Context(World world) : this(null, new List<IValue> { null }, new Dictionary<object, int>(), world) { }
        Context(object value, List<IValue> values, Dictionary<object, int> indices, World world)
        {
            Value = value;
            Values = values;
            Indices = indices;
            World = world;
        }

        public int Template(object value)
        {
            if (value is null) return 0;
            else if (Indices.TryGetValue(value, out var index)) return index;
            else if (World.Container.TryGet<ITemplater>(value.GetType(), out var templater))
            {
                index = Reserve(value);
                Values[index] = templater.Template(With(value));
                return index;
            }
            else return 0;
        }

        public int Reserve(object value)
        {
            var index = Values.Count;
            Indices[value] = index;
            Values.Add(default);
            return index;
        }

        public Context With(object value) => new Context(value, Values, Indices, World);
    }

    [Implementation(typeof(bool), typeof(Object<bool>)), Implementation(typeof(bool?), typeof(Object<bool?>)), Implementation(typeof(bool[]), typeof(Array<bool>))]
    [Implementation(typeof(char), typeof(Object<char>)), Implementation(typeof(char?), typeof(Object<char?>)), Implementation(typeof(char[]), typeof(Array<char>))]
    [Implementation(typeof(byte), typeof(Object<byte>)), Implementation(typeof(byte?), typeof(Object<byte?>)), Implementation(typeof(byte[]), typeof(Array<byte>))]
    [Implementation(typeof(sbyte), typeof(Object<sbyte>)), Implementation(typeof(sbyte?), typeof(Object<sbyte?>)), Implementation(typeof(sbyte[]), typeof(Array<sbyte>))]
    [Implementation(typeof(ushort), typeof(Object<ushort>)), Implementation(typeof(ushort?), typeof(Object<ushort?>)), Implementation(typeof(ushort[]), typeof(Array<ushort>))]
    [Implementation(typeof(short), typeof(Object<short>)), Implementation(typeof(short?), typeof(Object<short?>)), Implementation(typeof(short[]), typeof(Array<short>))]
    [Implementation(typeof(uint), typeof(Object<uint>)), Implementation(typeof(uint?), typeof(Object<uint?>)), Implementation(typeof(uint[]), typeof(Array<uint>))]
    [Implementation(typeof(int), typeof(Object<int>)), Implementation(typeof(int?), typeof(Object<int?>)), Implementation(typeof(int[]), typeof(Array<int>))]
    [Implementation(typeof(ulong), typeof(Object<ulong>)), Implementation(typeof(ulong?), typeof(Object<ulong?>)), Implementation(typeof(ulong[]), typeof(Array<ulong>))]
    [Implementation(typeof(long), typeof(Object<long>)), Implementation(typeof(long?), typeof(Object<long?>)), Implementation(typeof(long[]), typeof(Array<long>))]
    [Implementation(typeof(float), typeof(Object<float>)), Implementation(typeof(float?), typeof(Object<float?>)), Implementation(typeof(float[]), typeof(Array<float>))]
    [Implementation(typeof(double), typeof(Object<double>)), Implementation(typeof(double?), typeof(Object<double?>)), Implementation(typeof(double[]), typeof(Array<double>))]
    [Implementation(typeof(decimal), typeof(Object<decimal>)), Implementation(typeof(decimal?), typeof(Object<decimal?>)), Implementation(typeof(decimal[]), typeof(Array<decimal>))]
    [Implementation(typeof(string), typeof(Object<string>)), Implementation(typeof(string[]), typeof(Array<string>))]

    [Implementation(typeof(Entity), typeof(Null))]
    [Implementation(typeof(UnityEngine.Object), typeof(Unity))]
    [Implementation(typeof(object[]), typeof(Array<object>))]
    [Implementation(typeof(System.Array), typeof(Array))]
    [Implementation(typeof(object), typeof(Object))]
    public interface ITemplater : ITrait
    {
        IValue Template(in Context context);
    }

    public abstract class Templater<T> : ITemplater
    {
        public abstract IValue Template(in T value, in Context context);
        IValue ITemplater.Template(in Context context) =>
            context.Value is T value ? Template(value, context) : null;
    }

    public sealed class Object : ITemplater
    {
        public IValue Template(in Context context)
        {
            var value = context.Value;
            if (Utility.IsPrimitive(value)) return new Values.Primitive(value);

            var clone = CloneUtility.Shallow(value);
            var fields = clone.GetType().InstanceFields();
            var members = new List<(FieldInfo, int)>(fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var member = field.GetValue(clone);
                if (Utility.IsPrimitive(member)) continue;
                members.Add((field, context.Template(member)));
                // NOTE: don't keep references to values that will be overritten
                field.SetValue(clone, null);
            }
            return new Values.Object(clone, members.ToArray(), System.Array.Empty<(PropertyInfo, int)>());
        }
    }

    public sealed class Object<T> : Templater<T>
    {
        public override IValue Template(in T value, in Context context)
        {
            if (Utility.IsPrimitive(value)) return new Values.Primitive<T>(value);

            var clone = (object)CloneUtility.Shallow(value);
            var fields = clone.GetType().InstanceFields();
            var members = new List<(FieldInfo, int)>(fields.Length);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                var member = field.GetValue(clone);
                if (Utility.IsPrimitive(member)) continue;
                members.Add((field, context.Template(member)));
                // NOTE: don't keep references to values that will be overritten
                field.SetValue(clone, null);
            }
            return new Values.Object<T>((T)clone, members.ToArray(), System.Array.Empty<(PropertyInfo, int)>());
        }
    }

    public sealed class Array : Templater<System.Array>
    {
        public override IValue Template(in System.Array value, in Context context)
        {
            var clone = CloneUtility.Shallow(value);
            var items = new List<(int, int)>(value.Length);
            for (int i = 0; i < clone.Length; i++)
            {
                var item = clone.GetValue(i);
                if (Utility.IsPrimitive(item)) continue;
                items.Add((i, context.Template(item)));
                // NOTE: don't keep references to values that will be overritten
                clone.SetValue(null, i);
            }
            return new Values.Array(clone, items.ToArray());
        }
    }

    public sealed class Array<T> : Templater<T[]>
    {
        public override IValue Template(in T[] value, in Context context)
        {
            var clone = CloneUtility.Shallow(value);
            var items = new List<(int, int)>(value.Length);
            for (int i = 0; i < clone.Length; i++)
            {
                var item = clone[i];
                if (Utility.IsPrimitive(item)) continue;
                items.Add((i, context.Template(item)));
                // NOTE: don't keep references to values that will be overritten
                clone[i] = default;
            }
            return new Values.Array<T>(clone, items.ToArray());
        }
    }

    public sealed class Null : ITemplater
    {
        public IValue Template(in Context context) => null;
    }

    public sealed class Unity : Templater<UnityEngine.Object>
    {
        public override IValue Template(in UnityEngine.Object value, in Context context) =>
            new Values.Unity(value);
    }
}

namespace Entia.Experimental.Templating
{
    public static class Utility
    {
        public static bool IsPrimitive(object value)
        {
            if (value is null) return true;
            var type = value.GetType();
            return type.IsPrimitive || value is string || value is ICustomAttributeProvider;
        }
    }

    public static class Extensions
    {
        public static void Add<T>(this Container container, Templater<T> templater) =>
            container.Add<T, ITemplater>(templater);
        public static void Add<T>(this Container container, Instantiator<T> instantiator) where T : IValue =>
            container.Add<T, IInstantiator>(instantiator);

        public static Template<T> Template<T>(this World world, in T value)
        {
            var context = new Templaters.Context(world);
            var index = context.Template(value);
            return new Template<T>(index, context.Values.ToArray());
        }

        public static Template<Entity> Template(this World world, Entity entity)
        {
            var context = new Templaters.Context(world);
            var (index, resolve) = context.Descend(entity);
            resolve();
            return new Template<Entity>(index, context.Values.ToArray());
        }

        public static object Instantiate(this World world, in Template template)
        {
            var instances = new object[template.Values.Length];
            var context = new Instantiators.Context(template.Values, instances, world);
            for (int i = template.Values.Length - 1; i >= 0; i--)
            {
                var value = template.Values[i];
                if (value is null)
                    instances[i] = null;
                else if (world.Container.TryGet<IInstantiator>(value.GetType(), out var instantiator))
                    instantiator.Instantiate(context.With(value), out instances[i]);
                else
                    instances[i] = value;
            }
            return instances[template.Index];
        }

        public static T Instantiate<T>(this World world, in Template<T> template) =>
            world.Instantiate((Template)template) is T value ? value : default;

        static (int index, Action resolve) Descend(this Templaters.Context context, Entity entity)
        {
            var index = context.Reserve(entity);
            var children = context.World.Families().Children(entity);
            var components = context.World.Components().Get(entity).ToArray();
            var value = new Values.Entity(new int[children.Count], new int[components.Length]);
            var resolve = new Action(() =>
            {
                for (int i = 0; i < components.Length; i++)
                    value.Components[i] = context.Template(components[i]);
            });
            context.Values[index] = value;
            for (int i = 0; i < children.Count; i++)
            {
                var pair = context.Descend(children[i]);
                value.Children[i] = pair.index;
                resolve += pair.resolve;
            }
            return (index, resolve);
        }
    }

    public readonly struct Template<T>
    {
        public static readonly Template<T> Empty = new Template<T>(0, Dummy<IValue>.Read.Array.One);

        public static implicit operator Template(in Template<T> template) =>
            new Template(template.Index, template.Values);

        public readonly int Index;
        public readonly IValue[] Values;

        public Template(int index, params IValue[] values)
        {
            Index = index;
            Values = values;
        }
    }

    public readonly struct Template
    {
        public static readonly Template Empty = new Template(0, Dummy<IValue>.Read.Array.One);

        public readonly int Index;
        public readonly IValue[] Values;

        public Template(int index, params IValue[] values)
        {
            Index = index;
            Values = values;
        }

        public static Conflict[] Differentiate(Template left, Template right)
        {
            var setA = new HashSet<int>();
            var setB = new HashSet<int>();

            IEnumerable<Conflict> Conflicts(IValue valueA, IValue valueB, params string[] path)
            {
                var membersA = valueA?.Members.Where(member => setA.Add(member.Reference)).ToArray() ?? Dummy<Member>.Read.Array.Zero;
                var membersB = valueB?.Members.Where(member => setB.Add(member.Reference)).ToArray() ?? Dummy<Member>.Read.Array.Zero;

                if (membersA.Length == 0 && membersB.Length == 0)
                    return Equals(valueA?.Value, valueB?.Value) ?
                        Dummy<Conflict>.Read.Array.Zero :
                        new[] { new Conflict(valueA, valueB, path) };
                else
                {
                    return membersA.SelectMany(memberA => membersB
                        .Where(memberB => memberA.Name == memberB.Name)
                        .SelectMany(memberB => Conflicts(
                            left.Values[memberA.Reference],
                            right.Values[memberB.Reference],
                            path.Append(memberA.Name))));
                }
            }

            return Conflicts(left.Values[left.Index], right.Values[right.Index]).ToArray();
        }

        // - the omission of primitive fields/items prevents from diffing them properly
        // - define a function that resolves conflicts and combines templates
    }

    public readonly struct Conflict
    {
        public readonly IValue ValueA;
        public readonly IValue ValueB;
        public readonly string[] Path;

        public Conflict(IValue valueA, IValue valueB, params string[] path)
        {
            ValueA = valueA;
            ValueB = valueB;
            Path = path;
        }
    }
}