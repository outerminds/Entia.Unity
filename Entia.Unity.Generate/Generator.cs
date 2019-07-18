using Entia.Components;
using Entia.Core;
using Entia.Phases;
using Entia.Queryables;
using Entia.Systems;
using Entia.Unity.Editor;
using Entia.Unity.Generation;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Entia.Unity
{
    struct Void : IComponent, IResource, Entia.Queryables.IQueryable, Injectables.IInjectable, IPhase { }

    sealed class Context
    {
        public readonly CSharpCompilation Compilation;
        public readonly Dictionary<string, SyntaxTree> Trees;
        public readonly string Suffix;
        public readonly string Root;

        public readonly INamespaceSymbol Global;
        public readonly INamedTypeSymbol List;
        public readonly INamedTypeSymbol Nullable;

        public readonly INamedTypeSymbol World;
        public readonly INamedTypeSymbol Entity;
        public readonly INamedTypeSymbol Entities;
        public readonly INamedTypeSymbol Builders;
        public readonly INamedTypeSymbol IComponent;
        public readonly INamedTypeSymbol IResource;
        public readonly INamedTypeSymbol IQueryable;
        public readonly INamedTypeSymbol ISystem;
        public readonly INamedTypeSymbol IPhase;
        public readonly INamedTypeSymbol IEnabled;
        public readonly INamedTypeSymbol States;

        public readonly INamedTypeSymbol[] Groups;
        public readonly INamedTypeSymbol Write;
        public readonly INamedTypeSymbol Read;
        public readonly INamedTypeSymbol Maybe;

        public readonly INamedTypeSymbol Unity;
        public readonly INamedTypeSymbol WorldReference;
        public readonly INamedTypeSymbol WorldRegistry;
        public readonly INamedTypeSymbol EntityReference;
        public readonly INamedTypeSymbol EntityRegistry;
        public readonly INamedTypeSymbol ComponentReference;
        public readonly INamedTypeSymbol ResourceReference;

        public readonly INamedTypeSymbol Proxy;
        public readonly INamedTypeSymbol Serializable;
        public readonly INamedTypeSymbol AddComponentMenu;
        public readonly INamedTypeSymbol Generated;
        public readonly INamedTypeSymbol Generator;
        public readonly INamedTypeSymbol Preserve;
        public readonly INamedTypeSymbol SerializeField;
        public readonly INamedTypeSymbol Require;
        public readonly INamedTypeSymbol RequireComponent;
        public readonly INamedTypeSymbol Component;
        public readonly INamedTypeSymbol FormerlySerializedAsAttribute;

        public Context(string suffix, string root, CSharpCompilation compilation)
        {
            Suffix = suffix;
            Root = root;
            Compilation = compilation;
            Trees = compilation.SyntaxTrees
                .DistinctBy(tree => tree.FilePath)
                .ToDictionary(tree => tree.FilePath.Relative(root));
            Global = compilation.GlobalNamespace;

            List = Global.Type(typeof(List<>));
            Nullable = Global.Type(typeof(Nullable<>));

            World = Global.Type<World>();
            Entity = Global.Type<Entity>();
            Entities = Global.Type<Modules.Entities>();
            Builders = Global.Type<Modules.Builders>();
            IComponent = Global.Type<IComponent>();
            IResource = Global.Type<IResource>();
            IQueryable = Global.Type<Entia.Queryables.IQueryable>();
            ISystem = Global.Type<ISystem>();
            IPhase = Global.Type<IPhase>();
            IEnabled = Global.Type<IEnabled>();
            States = Global.Type<States>();

            Groups = Global.Types(typeof(Injectables.Group<>)).OrderBy(type => type.TypeParameters.Length).ToArray();
            Unity = Global.Type(true, nameof(Entia), nameof(Queryables), nameof(Unity));
            Write = Global.Type(typeof(Write<>));
            Read = Global.Type(typeof(Read<>));
            Maybe = Global.Type(typeof(Maybe<>));

            WorldReference = Global.Type(false, nameof(Entia), nameof(Unity), nameof(WorldReference));
            WorldRegistry = Global.Type(false, nameof(Entia), nameof(Unity), nameof(WorldRegistry));
            EntityReference = Global.Type(false, nameof(Entia), nameof(Unity), nameof(EntityReference));
            EntityRegistry = Global.Type(false, nameof(Entia), nameof(Unity), nameof(EntityRegistry));
            ComponentReference = Global.Type(true, nameof(Entia), nameof(Unity), nameof(ComponentReference<Void>));
            ResourceReference = Global.Type(true, nameof(Entia), nameof(Unity), nameof(ResourceReference<Void>));

            Proxy = Global.Type<ProxyAttribute>();
            Serializable = Global.Type<SerializableAttribute>();
            AddComponentMenu = Global.Type(false, "UnityEngine", "AddComponentMenu");
            Generated = Global.Type<GeneratedAttribute>();
            Generator = Global.Type<GeneratorAttribute>();
            Preserve = Global.Type<PreserveAttribute>();
            Require = Global.Type<RequireAttribute>();
            RequireComponent = Global.Type(false, "UnityEngine", "RequireComponent");
            Component = Global.Type(false, "UnityEngine", "Component");
            SerializeField = Global.Type(false, "UnityEngine", "SerializeField");
            FormerlySerializedAsAttribute = Global.Type(false, "UnityEngine", "Serialization", "FormerlySerializedAsAttribute");
        }
    }

    public sealed class Result
    {
        public (string[] type, string code)[] Generated;
        public (string[] from, string[] to)[] Renamed;
    }

    sealed class Extension : IEquatable<Extension>
    {
        public INamedTypeSymbol Outer;
        public ITypeSymbol Inner;
        public IEnumerable<string> Accesses = Array.Empty<string>();
        public bool Ref;
        public bool Readonly;
        public bool State;
        public int? Try;

        public bool Equals(Extension other) =>
            this == other ? true :
            other == null ? false :
            (Outer, Inner, Ref, Readonly, State, Try) == (other.Outer, other.Inner, other.Ref, other.Readonly, other.State, other.Try) &&
            Accesses.SequenceEqual(other.Accesses);
        public override bool Equals(object obj) => obj is Extension extension && Equals(extension);
        public override int GetHashCode() => (Outer, Inner, Ref, Readonly, State, Try).GetHashCode() ^ ArrayUtility.GetHashCode(Accesses.ToArray());
    }

    public static class Generator
    {
        static string Indentation(int depth) => new string(Enumerable.Repeat('\t', depth).ToArray());

        static string FormatPath(ITypeSymbol type) => FormatPath(type.Path().ToArray());

        static string FormatPath(string[] path) => "global::" + string.Join(".", path);

        static string FormatGenericPath(ITypeSymbol type)
        {
            var path = FormatPath(type);

            if (type is INamedTypeSymbol named)
            {
                if (named.TypeArguments.Length > 0)
                    path += $"<{string.Join(", ", named.TypeArguments.Select(FormatGenericPath))}>";
                else if (!named.TupleElements.IsDefaultOrEmpty)
                    path += $"<{string.Join(", ", named.TupleElements.Select(field => FormatGenericPath(field.Type)))}>";
            }

            return path;
        }

        static string FormatConstant(TypedConstant constant)
        {
            switch (constant.Kind)
            {
                case TypedConstantKind.Array: return $"new {FormatPath(constant.Type)} {{ {string.Join(", ", constant.Values.Select(FormatConstant))} }}";
                case TypedConstantKind.Primitive:
                    switch (Convert.GetTypeCode(constant.Value))
                    {
                        case TypeCode.UInt32: return $"{constant.ToCSharpString()}u";
                        case TypeCode.Int64: return $"{constant.ToCSharpString()}L";
                        case TypeCode.UInt64: return $"{constant.ToCSharpString()}uL";
                        case TypeCode.Single: return $"{constant.ToCSharpString()}f";
                        case TypeCode.Double: return $"{constant.ToCSharpString()}d";
                        case TypeCode.Decimal: return $"{constant.ToCSharpString()}m";
                        default: return constant.ToCSharpString();
                    }
                default: return constant.ToCSharpString();
            }
        }

        static (bool changed, string type, string from, string to) FormatConvertedType(
            ITypeSymbol type,
            Dictionary<ITypeSymbol, (ITypeSymbol type, string from, string to)> replacements,
            Dictionary<ITypeSymbol, (bool changed, string type, string from, string to)> conversions,
            Dictionary<INamedTypeSymbol, (string[] declaration, string[] extension)> declarations,
            Context context)
        {
            (bool changed, string type, string from, string to) Descend(ITypeSymbol current) =>
                FormatConvertedType(current, replacements, conversions, declarations, context);

            const string toEntia = nameof(Generation.GeneratorExtensions.ToEntia);
            const string fromEntia = nameof(Generation.GeneratorExtensions.FromEntia);

            if (conversions.TryGetValue(type, out var conversion)) return conversion;
            if (replacements.TryGetValue(type, out var replacement)) return conversions[type] = (true, Descend(replacement.type).type, replacement.from, replacement.to);

            conversion = (false, FormatGenericPath(type), "", "");
            switch (type)
            {
                case IArrayTypeSymbol array:
                    var element = Descend(array.ElementType);
                    if (element.changed)
                        conversion = (element.changed, $"{element.type}[]",
                            $"?.Map({{0}}, (item, state) => item{string.Format(element.from, "state")})",
                            $"?.Map(item => item{element.to})");
                    break;
                case INamedTypeSymbol list when list.Implements(context.List):
                    var item = Descend(list.TypeArguments[0]);
                    if (item.changed)
                        conversion = (item.changed, $"{FormatPath(list)}<{item.type}>",
                            $"?.Map({{0}}, (item, state) => item{string.Format(item.from, "state")})",
                            $"?.Map(item => item{item.to})");
                    break;
                case INamedTypeSymbol nullable when nullable.Implements(context.Nullable):
                    var argument = nullable.TypeArguments[0];
                    var underlying = Descend(argument);
                    if (underlying.changed)
                        conversion = (underlying.changed, $"{underlying.type}", $"?{underlying.from}", $"?{underlying.to}");
                    break;
            }

            if (type is INamedTypeSymbol named &&
                named.DeclaredAccessibility == Accessibility.Public &&
                named.SpecialType == SpecialType.None &&
                named.EnumUnderlyingType == null &&
                named.DelegateInvokeMethod == null &&
                !named.IsGenericType &&
                !named.IsAbstract &&
                !declarations.ContainsKey(named) &&
                !replacements.ContainsKey(named) &&
                named.GetAttributes().Any(attribute => attribute.AttributeClass.Implements(context.Proxy)) &&
                named.InstanceConstructors.Any(constructor => constructor.DeclaredAccessibility == Accessibility.Public && constructor.Parameters.Length == 0))
            {
                var fields = named.InstanceFields()
                    .Where(field => field.DeclaredAccessibility == Accessibility.Public && !field.IsReadOnly)
                    .ToArray();
                var proxy = string.Join("_", named.Path().Append("Proxy"));
                var from = $".{fromEntia}({{0}})";
                var to = $".{toEntia}({{0}})";
                var original = FormatGenericPath(named);
                var serializable = FormatPath(context.Serializable);

                conversions[type] = conversion = (true, proxy, from, to);
                declarations[named] = default;
                var converted = fields.Select(field => (field, conversion: Descend(field.Type))).ToArray();
                declarations[named] = (Declaration().ToArray(), Extension().ToArray());

                IEnumerable<string> Declaration()
                {
                    yield return $"[{serializable}]";
                    yield return $"public struct {proxy}";
                    yield return "{";
                    {
                        foreach (var pair in converted)
                        {
                            var attributes = string.Join(" ", pair.field.GetAttributes().Select(FormatAttribute));
                            if (!string.IsNullOrWhiteSpace(attributes)) yield return $"	{attributes}";
                            yield return $"	public {pair.conversion.type} {pair.field.Name};";
                        }
                    }
                    yield return "}";
                }

                IEnumerable<string> Extension()
                {
                    yield return $"public static class {proxy}Extensions";
                    yield return "{";
                    {
                        yield return $"	public static {original} {toEntia}(this {proxy} value) =>";
                        {
                            yield return $"		new {original}";
                            yield return "		{";
                            {
                                foreach (var pair in converted)
                                    yield return $"			{pair.field.Name} = value.{pair.field.Name}{string.Format(pair.conversion.to, "")},";
                            }
                            yield return "		};";
                        }
                        yield return $"	public static {proxy} {fromEntia}(this {original} value, {FormatPath(context.World)} world) =>";
                        {
                            yield return $"		new {proxy}";
                            yield return "		{";
                            {
                                foreach (var pair in converted)
                                    yield return $"			{pair.field.Name} = value.{pair.field.Name}{string.Format(pair.conversion.from, "world")},";
                            }
                            yield return "		};";
                        }
                    }
                    yield return "}";
                }
            }

            return conversions[type] = conversion;
        }

        static string FormatAttribute(AttributeData attribute)
        {
            var name = FormatPath(attribute.AttributeClass);
            var arguments = string.Join(", ", Enumerable.Concat(
                attribute.ConstructorArguments.Select(FormatConstant),
                attribute.NamedArguments.Select(argument => $"{argument.Key} = {FormatConstant(argument.Value)}")));
            return $"[{name}({arguments})]";
        }

        static string FormatField(int indent, IFieldSymbol field, string type, INamedTypeSymbol reference, Context context)
        {
            var indentation = Indentation(indent);
            var name = $"_{field.Name}";
            var attributes = string.Join(" ", field.GetAttributes()
                .Select(FormatAttribute)
                .Prepend($"[{FormatPath(context.SerializeField)}, {FormatPath(context.FormerlySerializedAsAttribute)}(nameof({field.Name}))]"));
            var @new = reference.Members(true).Any(member => member.Name == name);
            return
$@"{indentation}{attributes}
{indentation}{(@new ? "new " : "")}{type} {name};";
        }

        static string FormatProperty(int indent, IFieldSymbol field, string type, string data, string from, string to, INamedTypeSymbol reference, Context context)
        {
            var indentation = Indentation(indent);
            var name = $"this._{field.Name}";
            var @new = reference.Members(true).Any(member => member.Name == field.Name) ? "new " : "";
            var world = FormatPath(context.World);

            if (string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))
                return $"{indentation}{@new}public ref {type} {field.Name} => ref base.Get((ref {data} data) => ref data.{field.Name}, ref {name});";
            else
                return
$@"{indentation}{@new}public {type} {field.Name}
{indentation}{{
{indentation}	get => base.Get((ref {data} data, {world} world) => data.{field.Name}{string.Format(from, "world")}, {name});
{indentation}	set => base.Set((ref {data} data, {type} state, {world} _) => data.{field.Name} = state{string.Format(to, "")}, value, ref {name});
{indentation}}}";
        }

        static string FormatData(int indent, INamedTypeSymbol data, INamedTypeSymbol reference, Context context)
        {
            var replacements = new Dictionary<ITypeSymbol, (ITypeSymbol type, string from, string to)>
            {
                { context.Entity, (context.EntityReference, $".FromEntia({{0}})", $".ToEntia({{0}})") }
            };
            var conversions = new Dictionary<ITypeSymbol, (bool changed, string type, string from, string to)>();
            var declarations = new Dictionary<INamedTypeSymbol, (string[] declaration, string[] extension)>();
            var indentation = Indentation(indent);
            var name = data.Name;
            var fullName = FormatGenericPath(data);
            var proxies = $"{name}Proxies";
            var referenceName = FormatPath(reference);
            var fields = data.InstanceFields().ToArray();

            var converted = fields
                .Where(field => !field.IsReadOnly && field.DeclaredAccessibility == Accessibility.Public)
                .Select(field => (field, conversion: FormatConvertedType(field.Type, replacements, conversions, declarations, context)))
                .ToArray();
            var properties = string.Join(
                Environment.NewLine,
                converted.Select(pair => FormatProperty(indent + 1, pair.field, pair.conversion.type, fullName, pair.conversion.from, pair.conversion.to, reference, context)));
            var members = string.Join(
                Environment.NewLine,
                converted.Select(pair => FormatField(indent + 1, pair.field, pair.conversion.type, reference, context)));
            var initializers = string.Join(
                "," + Environment.NewLine,
                converted.Select(pair => $"{indentation}			{pair.field.Name} = this._{pair.field.Name}{string.Format(pair.conversion.to, "")}"));
            var setters = string.Join(
                Environment.NewLine,
                converted.Select(pair => $"{indentation}			this._{pair.field.Name} = value.{pair.field.Name}{string.Format(pair.conversion.from, "base.World")};"));
            var declaration = string.Join(
                Environment.NewLine + Environment.NewLine,
                declarations.Select(pair => string.Join(Environment.NewLine, pair.Value.declaration.Select(line => $"{indentation}	{line}"))));
            var extension = string.Join(
                Environment.NewLine + Environment.NewLine,
                declarations.Select(pair => string.Join(Environment.NewLine, pair.Value.extension.Select(line => $"{indentation}	{line}"))));

            var requireFields = fields
                .Where(field =>
                    field.Type.IsReferenceType && !field.Type.IsAbstract &&
                    field.GetAttributes().Any(attribute => attribute.AttributeClass.Implements(context.Require)))
                .ToArray();
            var requireAttributes = requireFields.Select(field => field.Type).Distinct().ToArray();
            var resetFields = string.Join(
                Environment.NewLine,
                requireFields.Select(field => $"{indentation}		this.{field.Name} = this.GetComponent<{FormatPath(field.Type)}>();"));
            var reset = requireAttributes.Length == 0 ? "" :
$@"{indentation}	void Reset()
{indentation}	{{
{resetFields}
{indentation}	}}";

            var proxy = string.IsNullOrWhiteSpace(declaration) && string.IsNullOrWhiteSpace(extension) ? "" :
$@"{indentation}using {proxies};

{indentation}namespace {proxies}
{indentation}{{
{declaration}

{extension}
{indentation}}}
";

            return
$@"{proxy}
{indentation}{FormatGenerated(data, context)}{FormatAddComponentMenu(data, context)}{FormatRequireComponent(context, requireAttributes)}
{indentation}public sealed partial class {name} : {referenceName}<{fullName}>
{indentation}{{
{properties}
{members}
{indentation}	public override {fullName} Raw
{indentation}	{{
{indentation}		get => new {fullName}
{indentation}		{{
{initializers}
{indentation}		}};
{indentation}		set
{indentation}		{{
{setters}
{indentation}		}}
{indentation}	}}
{reset}
{indentation}}}";
        }

        static string FormatGenerated(INamedTypeSymbol symbol, Context context)
        {
            var link = $@"{nameof(GeneratedAttribute.Link)} = ""{symbol.File().Relative(context.Root)}""";
            var type = $@"{nameof(GeneratedAttribute.Type)} = typeof({FormatPath(symbol)})";
            var path = $@"{nameof(GeneratedAttribute.Path)} = new string[] {{ {string.Join(", ", symbol.Path().Select(segment => $@"""{segment}"""))} }}";
            return $"[{FormatPath(context.Generated)}({type}, {link}, {path})]";
        }

        static string FormatAddComponentMenu(INamedTypeSymbol symbol, Context context) =>
            $@"[{FormatPath(context.AddComponentMenu)}(""{string.Join("", symbol.Path().SkipLast().Select(segment => segment + "/"))}{string.Join(".", symbol.Path())}"")]";

        static string FormatRequireComponent(Context context, params ITypeSymbol[] symbols) =>
            symbols.Length == 0 ? "" :
            $"[{string.Join(", ", symbols.Select(symbol => $"{FormatPath(context.RequireComponent)}(typeof({FormatPath(symbol)}))"))}]";

        static IEnumerable<Extension> QueryExtensions(ITypeSymbol query, Context context)
        {
            IEnumerable<Extension> Next(ITypeSymbol current, int depth)
            {
                if (current is INamedTypeSymbol named && named.Implements(context.IQueryable))
                {
                    var definition = named.OriginalDefinition;
                    if (context.Entity == definition)
                        yield return new Extension { Outer = named, Inner = named };
                    else if (context.Write == definition)
                        yield return new Extension
                        {
                            Outer = named,
                            Inner = named.TypeArguments[0],
                            Accesses = new string[] { nameof(Write<Void>.Value) },
                            State = !named.TypeArguments[0].Implements(context.IEnabled),
                            Ref = true
                        };
                    else if (context.Read == definition)
                        yield return new Extension
                        {
                            Outer = named,
                            Inner = named.TypeArguments[0],
                            Accesses = new string[] { nameof(Read<Void>.Value) },
                            State = !named.TypeArguments[0].Implements(context.IEnabled),
                            Ref = true,
                            Readonly = true
                        };
                    else if (context.Unity == definition)
                    {
                        yield return new Extension
                        {
                            Outer = named,
                            Inner = named.TypeArguments[0],
                            Accesses = new string[] { "Value" }
                        };
                    }
                    else if (context.Maybe == definition)
                    {
                        foreach (var field in named.InstanceFields().DistinctBy(field => field.Type))
                        {
                            foreach (var extension in Next(field.Type, depth + 1))
                            {
                                extension.Accesses = extension.Accesses.Prepend(field.Name);
                                extension.Try = extension.Try ?? depth;
                                yield return extension;
                            }
                        }
                    }
                    else
                    {
                        foreach (var field in named.InstanceFields().DistinctBy(field => field.Type))
                        {
                            foreach (var extension in Next(field.Type, depth + 1))
                            {
                                extension.Accesses = extension.Accesses.Prepend(field.Name);
                                yield return extension;
                            }
                        }
                    }
                }
            }

            return Next(query, 0);
        }

        static string FormatExtensionName(ITypeSymbol type) =>
            type is INamedTypeSymbol named && named.IsGenericType ?
            string.Join("", named.TypeArguments.Select(argument => argument.Name).Prepend(named.Name)) :
            type.Name;

        static IEnumerable<(string @return, string signature, string body)> FormatQueryExtensions(int indent, ITypeSymbol query, HashSet<ITypeSymbol> set, Context context)
        {
            const string item = nameof(item);
            const string value = nameof(value);
            const string success = nameof(success);
            const string state = nameof(state);
            var states = FormatPath(context.States);
            var indentation = Indentation(indent);
            if (set.Add(query))
            {
                var queryName = FormatGenericPath(query);
                foreach (var extension in QueryExtensions(query, context))
                {
                    var name = FormatExtensionName(extension.Inner);
                    var outerName = FormatGenericPath(extension.Outer);
                    var innerName = FormatGenericPath(extension.Inner);
                    var accessModifier = extension.Ref ? "ref " : "";
                    var returnModifier = extension.Ref ? extension.Readonly ? "ref readonly " : "ref " : "";

                    if (extension.Try is int @try)
                    {
                        var maybe = string.Join(".", extension.Accesses.Take(@try).Prepend(item));
                        var returnName = extension.Outer.OriginalDefinition == context.Unity ? innerName : outerName;

                        yield return ($"{indentation}public static bool ", $"Try{name}(in this {queryName} {item}, out {returnName} {value})", $@" => {maybe}.TryGet(out {value});");
                        yield return ($"{indentation}public static {returnModifier}{innerName} ", $"{name}(in this {queryName} {item}, out bool {success})", $@" => {accessModifier}{maybe}.Get(out {success});");

                        if (extension.State)
                            yield return ($"{indentation}public static {returnModifier}{innerName} ", $"{name}(in this {queryName} {item}, out bool {success}, out {states} {state})", $@" => {accessModifier}{maybe}.Get(out {success}, out {state});");
                    }
                    else
                    {
                        var access = string.Join(".", extension.Accesses.SkipLast().Prepend(item));
                        var accessValue = string.Join(".", extension.Accesses.Prepend(item));
                        yield return ($"{indentation}public static {returnModifier}{innerName} ", $"{name}(in this {queryName} {item})", $" => {accessModifier}{accessValue};");

                        if (extension.State)
                            yield return ($"{indentation}public static {returnModifier}{innerName} ", $"{name}(in this {queryName} {item}, out {states} {state})", $" => {accessModifier}{access}.Get(out {state});");

                        if (extension.Ref && !extension.Readonly)
                            yield return ($"{indentation}public static void ", $"{name}(in this {queryName} {item}, in {innerName} {value})", $" => {accessValue} = {value};");
                    }
                }
            }
        }

        static string FormatExtensions(INamedTypeSymbol system, HashSet<ITypeSymbol> set, Context context)
        {
            var @namespace = string.Join(".", system.Path().SkipLast());
            var hasNamespace = !string.IsNullOrWhiteSpace(@namespace);
            var indent = hasNamespace ? 1 : 0;
            var indentation = Indentation(indent);
            var extensions = string.Join(
                Environment.NewLine,
                system.Fields()
                    .Select(field => field.Type)
                    .OfType<INamedTypeSymbol>()
                    .Where(type => context.Groups.Any(group => group == type.OriginalDefinition))
                    .SelectMany(type => type.InstanceConstructors
                        .Where(constructor => constructor.Parameters.Length > 0)
                        .Select(constructor => constructor.Parameters[0].Type))
                    .OfType<INamedTypeSymbol>()
                    .SelectMany(type => FormatQueryExtensions(indent + 1, type.TypeArguments[0], set, context))
                    .DistinctBy(extension => extension.signature)
                    .Select(extension => $"{extension.@return}{extension.signature}{extension.body}"));
            var content =
$@"{indentation}using {nameof(Entia)}.{nameof(Entia.Queryables)};
{indentation}using {nameof(Entia)}.{nameof(Entia.Unity)};

{indentation}{FormatGenerated(system, context)}
{indentation}public static class {system.Name}Extensions
{indentation}{{
{extensions}
{indentation}}}";

            return hasNamespace ?
$@"namespace {@namespace}
{{
{content}
}}" : content;
        }

        static string FormatComponent(INamedTypeSymbol component, Context context)
        {
            var @namespace = string.Join(".", component.Path().SkipLast().Append(context.Suffix));
            return
$@"using {nameof(Entia)}.{nameof(Core)};
using {nameof(Entia)}.{nameof(Unity)}.{nameof(Generation)};

namespace {@namespace}
{{
{FormatData(1, component, context.ComponentReference, context)}
}}";
        }

        static string FormatResource(INamedTypeSymbol resource, Context context)
        {
            var @namespace = string.Join(".", resource.Path().SkipLast().Append(context.Suffix));
            return
$@"using {nameof(Entia)}.{nameof(Unity)}.{nameof(Generation)};

namespace {@namespace}
{{
{FormatData(1, resource, context.ResourceReference, context)}
}}";
        }

        static bool ValidateType(INamedTypeSymbol type, Context context, out string[] path)
        {
            path = default;

            if (type.IsGenericType ||
                type.IsAbstract ||
                type.IsImplicitlyDeclared ||
                type.IsStatic ||
                !type.CanBeReferencedByName ||
                type.DeclaredAccessibility != Accessibility.Public)
                return false;

            var attributes = type.GetAttributes();
            if (attributes.Any(attribute => attribute.AttributeClass.Implements(context.Generated))) return false;

            var generator = attributes.FirstOrDefault(attribute => attribute.AttributeClass.Implements(context.Generator));
            var ignore = generator?.NamedArguments
                .Where(argument => argument.Key == nameof(GeneratorAttribute.Ignore))
                .Any(argument => argument.Value.Value is bool value && value) ?? false;
            if (ignore) return false;

            path = generator?.NamedArguments
                .Where(argument => argument.Key == nameof(GeneratorAttribute.Path))
                .Select(argument => argument.Value.Values.Select(value => value.Value).OfType<string>().ToArray())
                .FirstOrDefault();

            if (path == null)
            {
                path = type.Path().ToArray();
                var file = type.File()?.Relative(context.Root);
                if (file?.Contains("Editor") is true) path = path.Prepend("Editor").ToArray();
                if (file?.Contains("Plugins") is true) path = path.Prepend("Plugins").ToArray();
            }

            return true;
        }

        static IEnumerable<(string[] type, string code)> FormatExtensions((INamedTypeSymbol type, string[] path)[] types, Context context)
        {
            var set = new HashSet<ITypeSymbol>();
            foreach (var (type, path) in types)
            {
                if (type.Implements(context.ISystem))
                    yield return (path.Prepend("Extensions").ToArray(), FormatExtensions(type, set, context));
            }
        }

        static IEnumerable<(string[] type, string code)> FormatTypes((INamedTypeSymbol type, string[] path)[] types, Context context)
        {
            foreach (var (type, path) in types)
            {
                if (type.Implements(context.IComponent)) yield return (path, FormatComponent(type, context));
                if (type.Implements(context.IResource)) yield return (path, FormatResource(type, context));
            }
        }

        static IEnumerable<(string[] from, string[] to)> Renamed((INamedTypeSymbol type, AttributeData attribute)[] types, Context context)
        {
            foreach (var (type, attribute) in types)
            {
                var arguments = attribute.NamedArguments.ToDictionary(pair => pair.Key, pair => pair.Value);
                if (arguments.TryGetValue(nameof(GeneratedAttribute.Type), out var typeConstant) &&
                    arguments.TryGetValue(nameof(GeneratedAttribute.Link), out var linkConstant) &&
                    arguments.TryGetValue(nameof(GeneratedAttribute.Path), out var pathConstant))
                {
                    var source = typeConstant.Value as INamedTypeSymbol;
                    var link = linkConstant.Value as string ?? "";
                    var path = pathConstant.Values.Select(value => value.Value).OfType<string>().ToArray();
                    var name = path.LastOrDefault() ?? "";

                    var isError = source == null || source.TypeKind == TypeKind.Error || source.Kind == SymbolKind.ErrorType;
                    if (isError && context.Trees.TryGetValue(link, out var tree) && context.Compilation.GetSemanticModel(tree) is SemanticModel model)
                    {
                        source = tree.GetRoot().DescendantNodes()
                            .OfType<TypeDeclarationSyntax>()
                            .Select(syntax => model.GetDeclaredSymbol(syntax))
                            .FirstOrDefault(symbol => symbol.Name == name);
                    }

                    if (source?.Path().ToArray() is string[] sourcePath && !path.SequenceEqual(sourcePath))
                        yield return (path, sourcePath);
                }
            }
        }

        public static async Task<Result> Generate(string suffix, string root, string[] inputFiles, string[] currentFiles, string[] assemblies, string[] defines)
        {
            var referencesTask = Task.Run(() => new[]
                {
                    typeof(Entity).Assembly.Location,
                    typeof(DrawGizmos).Assembly.Location,
                    typeof(PreserveAttribute).Assembly.Location,
                    typeof(RequireAttribute).Assembly.Location,
                }
                .Concat(assemblies)
                .Where(assembly => !string.IsNullOrEmpty(assembly))
                .DistinctBy(location => Path.GetFileNameWithoutExtension(location))
                .Select(assembly => MetadataReference.CreateFromFile(assembly))
                .ToArray());

            var options = CSharpParseOptions.Default.WithPreprocessorSymbols(defines);
            var inputTreesTask = Task.Run(() => inputFiles
                .AsParallel()
                .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), options, path: file))
                .ToArray());
            var currentTreesTask = Task.Run(() => currentFiles
                .AsParallel()
                .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), options, path: file))
                .ToArray());

            var references = await referencesTask;
            var inputTrees = await inputTreesTask;
            var currentTrees = await currentTreesTask;

            var compilation = CSharpCompilation.Create("", inputTrees.Concat(currentTrees), references);
            var types = compilation.AllTypes()
                .Where(type => !type.IsImplicitlyDeclared && type.CanBeReferencedByName)
                .ToArray();
            var context = new Context(suffix, root, compilation);
            var validTypes = types
                .AsParallel()
                .Select(type => ValidateType(type, context, out var path) ? (type, path) : default)
                .Where(pair => pair.path != null)
                .ToArray();
            var generatedTypes = types
                .AsParallel()
                .Select(type => (type, attribute: type.GetAttributes().FirstOrDefault(attribute => attribute.AttributeClass.Implements(context.Generated))))
                .Where(pair => pair.attribute != null)
                .ToArray();

            var formatTypesTask = Task.Run(() => FormatTypes(validTypes, context).ToArray());
            var formatExtensionsTask = Task.Run(() => FormatExtensions(validTypes, context).ToArray());
            var renamedTask = Task.Run(() => Renamed(generatedTypes, context).ToArray());

            var formatTypes = await formatTypesTask;
            var formatExtensions = await formatExtensionsTask;
            var renamed = await renamedTask;
            return new Result { Generated = formatTypes.Concat(formatExtensions).ToArray(), Renamed = renamed };
        }
    }
}