using Entia.Core;
using Entia.Dependencies;
using Entia.Modules.Group;
using Entia.Modules.Query;
using Entia.Phases;
using Entia.Queryables;
using Entia.Systems;
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
        public readonly INamedTypeSymbol Default;
        public readonly INamedTypeSymbol Preserve;
        public readonly INamedTypeSymbol SerializeField;
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

            Groups = Global.Types(typeof(Injectables.Group<>)).OrderBy(type => type.TypeParameters.Length).ToArray();
            Unity = Global.Type(true, nameof(Entia), nameof(Queryables), "Unity");
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
            Default = Global.Type<DefaultAttribute>();
            Preserve = Global.Type<PreserveAttribute>();
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
        public int? Try;

        public bool Equals(Extension other) =>
            this == other ? true :
            other == null ? false :
            (Outer, Inner, Ref, Readonly, Try) == (other.Outer, other.Inner, other.Ref, other.Readonly, other.Try) &&
            Accesses.SequenceEqual(other.Accesses);
        public override bool Equals(object obj) => obj is Extension extension && Equals(extension);
        public override int GetHashCode() => (Outer, Inner, Ref, Readonly, Try).GetHashCode() ^ ArrayUtility.GetHashCode(Accesses.ToArray());
    }

    public static class Generator
    {
        static string Indentation(int depth) => new string(Enumerable.Repeat('\t', depth).ToArray());

        static string FormatPath(ITypeSymbol type) => FormatPath(type.Path().ToArray());

        static string FormatPath(string[] path) => "global::" + string.Join(".", path);

        static string FormatGenericPath(ITypeSymbol type)
        {
            var path = FormatPath(type);

            if (type is INamedTypeSymbol named && named.TypeArguments.Length > 0)
                path += $"<{string.Join(", ", named.TypeArguments.Select(FormatGenericPath))}>";

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

            switch (type)
            {
                case IArrayTypeSymbol array:
                    {
                        var element = Descend(array.ElementType);
                        conversion = (element.changed, $"{element.type}[]", $"?.Select(item => item{element.from}).ToArray()", $"?.Select(item => item{element.to}).ToArray()");
                        break;
                    }
                case INamedTypeSymbol list when list.Implements(context.List):
                    {
                        var item = Descend(list.TypeArguments[0]);
                        conversion = (item.changed, $"{FormatPath(list)}<{item.type}>", $"?.Select(item => item{item.from}).ToList()", $"?.Select(item => item{item.to}).ToList()");
                        break;
                    }
                case INamedTypeSymbol nullable when nullable.Implements(context.Nullable):
                    {
                        var argument = nullable.TypeArguments[0];
                        var underlying = Descend(argument);
                        if (replacements.TryGetValue(argument, out var current) && current.type.IsReferenceType)
                            conversion = (underlying.changed, $"{underlying.type}", $"?{underlying.from}", $"?{underlying.to}");
                        else
                            conversion = (underlying.changed, $"{FormatPath(nullable)}<{underlying.type}>", $"?{underlying.from}", $"?{underlying.to}");
                        break;
                    }
                default: conversion = (false, FormatGenericPath(type), "", ""); break;
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
            var constant = field.GetAttributes()
                .Where(attribute => attribute.AttributeClass.Implements(context.Default))
                .SelectMany(attribute => attribute.ConstructorArguments)
                .Where(argument => argument.Type.Implements(field.Type))
                .Select(FormatConstant)
                .FirstOrDefault();
            var @default = string.IsNullOrWhiteSpace(constant) ? "" : $" = {constant}";
            var @new = reference.Members(true).Any(member => member.Name == name);
            return
$@"{indentation}{attributes}
{indentation}{(@new ? "new " : "")}{type} {name}{@default};";
        }

        static string FormatProperty(int indent, IFieldSymbol field, string type, string from, string to, INamedTypeSymbol reference)
        {
            var indentation = Indentation(indent);
            var name = $"_{field.Name}";
            var @new = reference.Members(true).Any(member => member.Name == field.Name);

            if (string.IsNullOrWhiteSpace(from) && string.IsNullOrWhiteSpace(to))
                return $"{indentation}{(@new ? "new " : "")}public ref {type} {field.Name} => ref this.{name};";
            else
                return
$@"{indentation}{(@new ? "new " : "")}public {type} {field.Name}
{indentation}{{
{indentation}	get => this.{name}{string.Format(to, "")};
{indentation}	set => this.{name} = value{string.Format(from, "this.World")};
{indentation}}}";
        }

        static string FormatData(int indent, INamedTypeSymbol data, INamedTypeSymbol reference, string property, Context context)
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

            var converted = data.InstanceFields()
                .Where(field => !field.IsReadOnly && field.DeclaredAccessibility == Accessibility.Public)
                .Select(field => (field, conversion: FormatConvertedType(field.Type, replacements, conversions, declarations, context)))
                .ToArray();
            var properties = string.Join(
                Environment.NewLine,
                converted.Select(pair => FormatProperty(indent + 1, pair.field, FormatGenericPath(pair.field.Type), pair.conversion.from, pair.conversion.to, reference)));
            var fields = string.Join(
                Environment.NewLine,
                converted.Select(pair => FormatField(indent + 1, pair.field, pair.conversion.type, reference, context)));
            var initializers = string.Join(
                "," + Environment.NewLine,
                converted.Select(pair => $"{indentation}			{pair.field.Name} = this.{pair.field.Name}"));
            var setters = string.Join(
                Environment.NewLine,
                converted.Select(pair => $"{indentation}			this.{pair.field.Name} = value.{pair.field.Name};"));
            var declaration = string.Join(
                Environment.NewLine + Environment.NewLine,
                declarations.Select(pair => string.Join(Environment.NewLine, pair.Value.declaration.Select(line => $"{indentation}	{line}"))));
            var extension = string.Join(
                Environment.NewLine + Environment.NewLine,
                declarations.Select(pair => string.Join(Environment.NewLine, pair.Value.extension.Select(line => $"{indentation}	{line}"))));
            var proxy = string.IsNullOrWhiteSpace(declaration) && string.IsNullOrWhiteSpace(extension) ? "" :
$@"{indentation}using {proxies};

{indentation}namespace {proxies}
{indentation}{{
{declaration}

{extension}
{indentation}}}
";

            return
$@"{indentation}using System.Linq;
{proxy}
{indentation}{FormatGenerated(data, context)}
{indentation}{FormatAddComponentMenu(data, context)}
{indentation}public sealed partial class {name} : {referenceName}<{fullName}>
{indentation}{{
{properties}
{fields}
{indentation}	public override {fullName} {property}
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

        static IEnumerable<Extension> QueryExtensions(ITypeSymbol query, Context context)
        {
            IEnumerable<Extension> Next(ITypeSymbol current, int depth)
            {
                if (current is INamedTypeSymbol named && named.Implements(context.IQueryable))
                {
                    var definition = named.OriginalDefinition;
                    if (context.Write == definition)
                        yield return new Extension
                        {
                            Outer = named,
                            Inner = named.TypeArguments[0],
                            Accesses = new string[] { nameof(Write<Void>.Value) },
                            Ref = true
                        };
                    else if (context.Read == definition)
                        yield return new Extension
                        {
                            Outer = named,
                            Inner = named.TypeArguments[0],
                            Accesses = new string[] { nameof(Read<Void>.Value) },
                            Ref = true,
                            Readonly = true
                        };
                    else if (context.Unity == definition)
                    {
                        yield return new Extension
                        {
                            Outer = named,
                            Inner = named.TypeArguments[0],
                            Accesses = new string[] { "Value" },
                            Ref = false,
                            Readonly = false
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

        static IEnumerable<string> FormatQueryExtensions(int indent, ITypeSymbol query, HashSet<ITypeSymbol> set, Context context)
        {
            const string item = "item";
            const string value = "value";
            var indentation = Indentation(indent);
            if (set.Add(query))
            {
                var queryName = FormatGenericPath(query);
                foreach (var extension in QueryExtensions(query, context))
                {
                    var name = FormatExtensionName(extension.Inner);
                    var outerName = FormatGenericPath(extension.Outer);
                    var innerName = FormatGenericPath(extension.Inner);
                    if (extension.Try.HasValue)
                    {
                        var @try = extension.Try.Value;
                        var maybeValue = string.Join(".", extension.Accesses.Take(@try).Prepend(item).Append(nameof(Maybe<Void>.Value)));
                        var maybeHas = string.Join(".", extension.Accesses.Take(@try).Prepend(item).Append(nameof(Maybe<Void>.Has)));
                        var (accessSuffix, returnName, returnHas) =
                            extension.Outer.OriginalDefinition == context.Unity ?
                            ($".Value", innerName, $"{value} != null") :
                            ("", outerName, "true");

                        yield return
$@"{indentation}public static bool Try{name}(in this {queryName} {item}, out {returnName} {value})
{indentation}{{
{indentation}	if ({maybeHas})
{indentation}	{{
{indentation}		{value} = {maybeValue}{accessSuffix};
{indentation}		return {returnHas};
{indentation}	}}
{indentation}	
{indentation}	{value} = default;
{indentation}	return false;
{indentation}}}";
                    }
                    else
                    {
                        var access = string.Join(".", extension.Accesses.Prepend(item));
                        var accessModifier = extension.Ref ? "ref " : "";
                        var returnModifier = extension.Ref ? extension.Readonly ? "ref readonly " : "ref " : "";

                        yield return $"{indentation}public static {returnModifier}{innerName} {name}(in this {queryName} {item}) => {accessModifier}{access};";

                        if (extension.Ref && !extension.Readonly)
                            yield return $"{indentation}public static void {name}(in this {queryName} {item}, in {innerName} {value}) => {access} = {value};";
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
                    .SelectMany(type => FormatQueryExtensions(indent + 1, type.TypeArguments[0], set, context)));
            var content =
$@"{indentation}{FormatGenerated(system, context)}
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
$@"using {nameof(Entia)}.{nameof(Unity)}.{nameof(Generation)};

namespace {@namespace}
{{
{FormatData(1, component, context.ComponentReference, "Component", context)}
}}";
        }

        static string FormatResource(INamedTypeSymbol resource, Context context)
        {
            var @namespace = string.Join(".", resource.Path().SkipLast().Append(context.Suffix));
            return
$@"using {nameof(Entia)}.{nameof(Unity)}.{nameof(Generation)};

namespace {@namespace}
{{
{FormatData(1, resource, context.ResourceReference, "Resource", context)}
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
                .FirstOrDefault() ?? type.Path().ToArray();

            return true;
        }

        static IEnumerable<(string[] type, string code)> FormatExtensions((INamedTypeSymbol type, string[] path)[] types, Context context)
        {
            var set = new HashSet<ITypeSymbol>();
            foreach (var (type, path) in types)
            {
                if (type.Implements(context.ISystem))
                    yield return (type.Path().Prepend("Extensions").ToArray(), FormatExtensions(type, set, context));
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

        public static async Task<Result> Generate(string suffix, string root, string[] inputFiles, string[] currentFiles, string[] assemblies)
        {
            var referencesTask = Task.Run(() => new[]
            {
                typeof(Entity).Assembly.Location,
                typeof(PreserveAttribute).Assembly.Location,
                typeof(DefaultAttribute).Assembly.Location,
            }
                .Concat(assemblies)
                .Where(assembly => !string.IsNullOrEmpty(assembly))
                .DistinctBy(location => Path.GetFileNameWithoutExtension(location))
                .Select(assembly => MetadataReference.CreateFromFile(assembly))
                .ToArray());

            var inputTreesTask = Task.Run(() => inputFiles
                .AsParallel()
                .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file))
                .ToArray());
            var currentTreesTask = Task.Run(() => currentFiles
                .AsParallel()
                .Select(file => CSharpSyntaxTree.ParseText(File.ReadAllText(file), path: file))
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