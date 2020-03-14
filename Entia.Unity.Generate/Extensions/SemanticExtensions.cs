﻿using Entia.Core;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Entia.Unity
{
    public static class SemanticExtensions
    {
        public static string File(this ISymbol symbol) =>
            symbol.Locations.Select(location => location.GetLineSpan().Path).FirstOrDefault(path => !string.IsNullOrWhiteSpace(path));

        public static IEnumerable<string> Path(this INamespaceOrTypeSymbol symbol)
        {
            if (symbol.ContainingSymbol is INamespaceOrTypeSymbol parent)
                foreach (var path in parent.Path()) yield return path;

            switch (symbol)
            {
                case IArrayTypeSymbol array:
                    var path = array.ElementType.Path().ToArray();
                    var indexer = $"[{string.Join("", Enumerable.Repeat(",", array.Rank - 1))}]";
                    path[path.Length - 1] += indexer;
                    foreach (var part in path) yield return part;
                    break;
                case INamedTypeSymbol tuple when !tuple.TupleElements.IsDefaultOrEmpty:
                    yield return nameof(ValueTuple);
                    break;
                case INamespaceSymbol @namespace when !@namespace.IsGlobalNamespace:
                    foreach (var part in @namespace.ConstituentNamespaces) yield return part.Name;
                    break;
                case ITypeSymbol type: yield return type.Name; break;
                default: break;
            }
        }

        public static IEnumerable<IFieldSymbol> Fields(this ITypeSymbol symbol, bool recursive = false) =>
            symbol.Members(recursive).OfType<IFieldSymbol>();

        public static IEnumerable<IFieldSymbol> InstanceFields(this ITypeSymbol symbol) => symbol.GetMembers()
            .OfType<IFieldSymbol>()
            .Where(field =>
                !field.IsImplicitlyDeclared &&
                !field.IsStatic &&
                !field.IsConst &&
                field.CanBeReferencedByName);

        public static IEnumerable<ISymbol> Members(this ITypeSymbol symbol, bool recursive = false)
        {
            var members = symbol.GetMembers();
            if (recursive) return members.Concat(symbol.Bases().SelectMany(@base => @base.Members()));
            else return members;
        }

        public static INamespaceSymbol Namespace(this SemanticModel model, string name) =>
            model.LookupNamespacesAndTypes(0, name: name).OfType<INamespaceSymbol>().FirstOrDefault();

        public static INamespaceSymbol Namespace(this INamespaceSymbol symbol, string name) =>
            symbol.GetNamespaceMembers().FirstOrDefault(member => member.Name == name);

        public static IEnumerable<INamespaceSymbol> Namespaces(this INamespaceSymbol symbol) =>
            symbol.GetMembers().OfType<INamespaceSymbol>();

        public static IEnumerable<INamespaceSymbol> Namespaces(this INamespaceSymbol symbol, string name) =>
            symbol.Namespaces().Where(@namespace => @namespace.Name == name);

        public static IEnumerable<ISymbol> Ancestors(this ISymbol symbol) =>
            symbol == null ? Enumerable.Empty<ISymbol>() : Ancestors(symbol.ContainingSymbol).Prepend(symbol);

        public static INamedTypeSymbol Type<T>(this INamespaceOrTypeSymbol symbol) => symbol.Type(typeof(T));
        public static INamedTypeSymbol Type(this INamespaceOrTypeSymbol symbol, Type type) => symbol.Types(type).FirstOrDefault();
        public static INamedTypeSymbol Type(this INamespaceOrTypeSymbol symbol, bool generic, params string[] path) => symbol.Types(generic, path).FirstOrDefault();
        public static INamedTypeSymbol Type(this INamespaceOrTypeSymbol symbol, params string[] path) => symbol.Types(path).FirstOrDefault();
        public static IEnumerable<INamedTypeSymbol> Types<T>(this INamespaceOrTypeSymbol symbol) => symbol.Types(typeof(T));
        public static IEnumerable<INamedTypeSymbol> Types(this INamespaceOrTypeSymbol symbol, Type type) => symbol.Types(type.IsGenericType, type.Path().ToArray());
        public static IEnumerable<INamedTypeSymbol> Types(this INamespaceOrTypeSymbol symbol, bool generic, params string[] path) =>
            symbol.Types(path).Where(type => type.IsGenericType == generic);
        public static IEnumerable<INamedTypeSymbol> Types(this INamespaceOrTypeSymbol symbol, params string[] path)
        {
            var current = new INamespaceOrTypeSymbol[] { symbol };
            foreach (var segment in path)
                current = current.SelectMany(child => child.GetMembers(segment)).OfType<INamespaceOrTypeSymbol>().ToArray();
            return current.OfType<INamedTypeSymbol>();
        }

        public static INamedTypeSymbol Type(this INamespaceOrTypeSymbol symbol, string name) =>
            symbol.Types(name).FirstOrDefault();

        public static INamedTypeSymbol Type(this INamespaceOrTypeSymbol symbol, string name, Func<INamedTypeSymbol, bool> filter) =>
            symbol.Types(name).FirstOrDefault(filter);

        public static INamedTypeSymbol GenericType(this INamespaceOrTypeSymbol symbol, string name) =>
            symbol.Type(name, type => type.IsGenericType);

        public static INamedTypeSymbol GenericType(this INamespaceOrTypeSymbol symbol, string name, int parameters) =>
            symbol.Type(name, type => type.IsGenericType && type.TypeArguments.Length == parameters);

        public static IEnumerable<INamedTypeSymbol> Types(this INamespaceOrTypeSymbol symbol, string name) =>
            symbol.GetTypeMembers().Where(member => member.Name == name);

        public static IEnumerable<INamedTypeSymbol> AllTypes(this Compilation compilation)
        {
            IEnumerable<INamedTypeSymbol> Descend(INamespaceOrTypeSymbol symbol)
            {
                switch (symbol)
                {
                    case INamespaceSymbol @namespace: return @namespace.GetMembers().SelectMany(Descend);
                    case INamedTypeSymbol type: return type.GetTypeMembers().SelectMany(Descend).Prepend(type);
                    default: return Enumerable.Empty<INamedTypeSymbol>();
                }
            }

            return Descend(compilation.GlobalNamespace);
        }

        public static IEnumerable<INamedTypeSymbol> Types(this SemanticModel model)
        {
            IEnumerable<INamedTypeSymbol> Descend(INamespaceOrTypeSymbol symbol)
            {
                switch (symbol)
                {
                    case INamespaceSymbol @namespace: return @namespace.GetMembers().SelectMany(Descend);
                    case INamedTypeSymbol type: return type.GetTypeMembers().SelectMany(Descend).Prepend(type);
                    default: return Enumerable.Empty<INamedTypeSymbol>();
                }
            }

            return model.LookupNamespacesAndTypes(0).OfType<INamespaceOrTypeSymbol>().SelectMany(Descend);
        }

        public static IEnumerable<INamedTypeSymbol> Bases(this ITypeSymbol symbol)
        {
            var current = symbol.BaseType;
            while (current != null)
            {
                yield return current;
                current = current.BaseType;
            }
        }

        public static bool IsDefaultConstructor(this IMethodSymbol method) =>
            method.MethodKind == MethodKind.Constructor || method.MethodKind == MethodKind.StaticConstructor ?
            method.IsImplicitlyDeclared : false;

        public static bool Implements(this ITypeSymbol symbol, ITypeSymbol type) =>
            SymbolEqualityComparer.Default.Equals(symbol, type) ||
            SymbolEqualityComparer.Default.Equals(symbol.OriginalDefinition, type) ||
            symbol.AllInterfaces.Any(@interface =>
                SymbolEqualityComparer.Default.Equals(@interface, type) ||
                SymbolEqualityComparer.Default.Equals(@interface.OriginalDefinition, type)) ||
            (symbol.BaseType is INamedTypeSymbol @base && @base.Implements(type));
    }
}