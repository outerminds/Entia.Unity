﻿using Entia.Analysis;
using Entia.Build;
using Entia.Components;
using Entia.Core;
using Entia.Dependencies;
using Entia.Dependency;
using Entia.Injectables;
using Entia.Modules;
using Entia.Modules.Component;
using Entia.Modules.Group;
using Entia.Modules.Message;
using Entia.Nodes;
using Entia.Phases;
using Entia.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class WorldExtensions
    {
        static Type[] Concretes => _concretes ?? (_concretes = TypeUtility.AllTypes
            .Where(ComponentUtility.IsConcrete)
            .OrderBy(type => type.FullName)
            .ToArray());
        static Type[] _concretes;

        public static object ShowValue(this World world, string label, object value, params string[] path)
        {
            switch (value)
            {
                case World _: LayoutUtility.Label(label); break;
                case Entity entity: world.ShowEntity(label, entity, path); break;
                case Controller controller: world.ShowController(controller, new Dictionary<IRunner, TimeSpan>(), false, path); break;
                case IGroup group: world.ShowGroup(label, group, path); break;
                case IEmitter emitter: world.ShowEmitter(label, emitter); break;
                case IReceiver receiver: world.ShowReceiver(label, receiver); break;
                case IReaction reaction: world.ShowReaction(label, reaction); break;
                case IResource resource: world.ShowResource(label, resource, path); break;
                case IInjectable injectable: world.ShowInjectable(label, injectable, path); break;
                default: value = LayoutUtility.Object(label, value, value?.GetType() ?? typeof(object), path); break;
            }

            return value;
        }

        public static object ShowValues(this World world, string label, IEnumerable values, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                values.Cast<object>().ToArray(),
                (item, index) => world.ShowValue(item.ToString(), item, path.Append(index.ToString()).ToArray()),
                values.GetType(),
                path);

        public static void ShowInjectable(this World world, string label, IInjectable injectable, params string[] path)
        {
            Result<T> GetValue<T>(string name) => Result.Try(() =>
                injectable.GetType().GetField(name, TypeUtility.Instance).GetValue(injectable)).Cast<T>();

            var type = injectable.GetType();
            if (injectable is AllEntities || injectable is AllEntities.Read) world.ShowEntities(label, world.Entities(), path);
            else if (injectable is AllComponents) world.ShowComponents(label, world.Entities(), path);
            else if (TypeUtility.Is(injectable, typeof(Resource<>), definition: true) || TypeUtility.Is(injectable, typeof(Resource<>.Read), definition: true))
            {
                var resource = type.GetGenericArguments()[0];
                world.ShowResource(label, world.Resources().Get(resource), path);
            }
            else if (TypeUtility.Is(injectable, typeof(Components<>), definition: true) || TypeUtility.Is(injectable, typeof(Components<>.Read), definition: true))
            {
                var component = type.GetGenericArguments()[0];
                var pairs = world.Entities()
                    .Select(entity => world.Components().TryGet(entity, component, out var current) ? (entity, component: current) : default)
                    .Where(pair => pair.component != null);
                world.ShowComponents(label, pairs, component, path);
            }
            else if (TypeUtility.Is(injectable, typeof(Injectables.Emitter<>), definition: true) && GetValue<IEmitter>("_emitter").TryValue(out var emitter))
                world.ShowEmitter(label, emitter);
            else if (TypeUtility.Is(injectable, typeof(Injectables.Receiver<>), definition: true) && GetValue<IReceiver>("_receiver").TryValue(out var receiver))
                world.ShowReceiver(label, receiver);
            else if (TypeUtility.Is(injectable, typeof(Injectables.Reaction<>), definition: true) && GetValue<IReaction>("_reaction").TryValue(out var reaction))
                world.ShowReaction(label, reaction);
            else if (GetValue<IGroup>("_group").TryValue(out var group))
                world.ShowGroup(label, group, path);
            else LayoutUtility.Label(label);
        }

        public static bool ShowEntityFoldout(this World world, string label, Entity entity, Type type, params string[] path)
        {
            using (LayoutUtility.Horizontal())
            {
                var expanded = world.Components().TryGet<Unity<EntityReference>>(entity, out var component) ?
                    LayoutUtility.PingFoldout(label, component.Value, type, path) :
                    LayoutUtility.Foldout(label, type, path);
                using (LayoutUtility.Disable(!EditorApplication.isPlaying))
                {
                    if (LayoutUtility.PlusButton())
                    {
                        var menu = new GenericMenu();
                        foreach (var concrete in Concretes)
                        {
                            var content = new GUIContent(string.Join("/", concrete.Path().SkipLast().Append(concrete.Format())));
                            if (world.Components().Has(entity, concrete)) menu.AddDisabledItem(content, false);
                            else menu.AddItem(content, false, () => world.Components().Set(entity, concrete));
                        }
                        menu.ShowAsContext();
                    }

                    if (LayoutUtility.MinusButton()) world.Entities().Destroy(entity);
                    return expanded;
                }
            }
        }

        public static void ShowEntity(this World world, string label, Entity entity, params string[] path)
        {
            path = path.Append(entity.ToString()).ToArray();
            using (LayoutUtility.Disable(!world.Entities().Has(entity)))
            {
                LayoutUtility.ChunksFoldout(
                    label,
                    world.Components().Get(entity).ToArray(),
                    (value, index) => world.ShowComponent(value.GetType().Format(), entity, value, path.Append(index.ToString()).ToArray()),
                    entity.GetType(),
                    path: path,
                    foldout: data => world.ShowEntityFoldout(data.label, entity, data.type, data.path));
            }
        }

        public static void ShowEntities(this World world, string label, IEnumerable<Entity> entities, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                entities.ToArray(),
                (entity, index) => world.ShowEntity(entity.ToString(world), entity, path.Append(index.ToString()).ToArray()),
                entities.GetType(),
                path.Append(entities.GetType().FullName).ToArray());

        public static void ShowComponents(this World world, string label, IEnumerable<Entity> entities, params string[] path)
        {
            var groups = entities
                .SelectMany(entity => world.Components().Get(entity).Select(component => (entity, component)))
                .GroupBy(pair => pair.component.GetType())
                .Select(group => (group.Key, group.ToArray()))
                .ToArray();

            LayoutUtility.ChunksFoldout(
                label,
                groups,
                (group, index) => world.ShowComponents(group.Key.Format(), group.Item2, group.Key, path.Append(index.ToString()).ToArray()),
                entities.GetType(),
                path);
        }

        public static void ShowComponents(this World world, string label, IEnumerable<(Entity entity, IComponent component)> components, Type type, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                components.ToArray(),
                (pair, index2) => world.ShowComponent(pair.entity.ToString(world), pair.entity, pair.component, path.Append(index2.ToString()).ToArray()),
                type,
                path.Append(type.FullName).ToArray());

        public static void ShowComponent(this World world, string label, Entity entity, IComponent component, params string[] path)
        {
            var type = component.GetType();

            using (LayoutUtility.Disable(!EditorApplication.isPlaying))
            using (LayoutUtility.Horizontal())
            {
                using (LayoutUtility.Vertical())
                {
                    EditorGUI.BeginChangeCheck();
                    component = LayoutUtility.Object(label, component, type, path.Append(type.FullName, entity.ToString()).ToArray()) as IComponent;
                    if (EditorGUI.EndChangeCheck() && component is IComponent current) world.Components().Set(entity, current);
                }

                if (!type.Is<IEnabled>())
                {
                    EditorGUI.BeginChangeCheck();
                    var state = world.Components().State(entity, type);
                    var enabled = LayoutUtility.Toggle(state == States.Enabled);
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (enabled) world.Components().Enable(entity, type);
                        else world.Components().Disable(entity, type);
                    }
                }

                if (LayoutUtility.MinusButton()) world.Components().Remove(entity, type);
            }
        }

        public static void ShowGroups(this World world, IEnumerable<IGroup> groups, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                nameof(Modules.Groups),
                groups.ToArray(),
                (group, index) => world.ShowGroup(group.Type.Format(), group, path.Append(index.ToString()).ToArray()),
                typeof(Modules.Groups));

        public static void ShowGroup(this World world, string label, IGroup group, params string[] path) =>
            world.ShowEntities(label, group.Entities, path);

        public static void ShowResource(this World world, string label, IResource resource, params string[] path)
        {
            var type = resource.GetType();
            using (LayoutUtility.Disable(!EditorApplication.isPlaying))
            {
                EditorGUI.BeginChangeCheck();
                resource = LayoutUtility.Object(label, resource, type, path) as IResource;
                if (EditorGUI.EndChangeCheck() && resource is IResource current) world.Resources().Set(current);
            }
        }

        public static void ShowEmitters(this World world, string label, IEnumerable<IEmitter> emitters, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                emitters.ToArray(),
                (emitter, _) => world.ShowEmitter(emitter.Type.Format(), emitter),
                emitters.GetType(),
                path);

        public static void ShowResources(this World world, string label, IEnumerable<IResource> resources, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                resources.ToArray(),
                (resource, index) => world.ShowResource(resource.GetType().Format(), resource, path.Append(index.ToString()).ToArray()),
                typeof(Modules.Resources),
                path);

        public static void ShowFamilies(this World world, string label, params string[] path) =>
            world.ShowFamilies(label, world.Families().Roots(), path);

        public static void ShowFamilies(this World world, string label, IEnumerable<Entity> roots, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                roots.ToArray(),
                (entity, index) => world.ShowFamily(entity.ToString(world), entity, path.Append(index.ToString()).ToArray()),
                typeof(Modules.Families),
                path);

        public static void ShowFamily(this World world, string label, Entity parent, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                world.Families().Children(parent).ToArray(),
                (child, index) => world.ShowFamily(child.ToString(world), child, path.Append(index.ToString()).ToArray()),
                typeof(Modules.Families),
                path,
                foldout: data => world.ShowEntityFoldout(data.label, parent, data.type, data.path));

        public static void ShowEmitter(this World _, string label, IEmitter emitter)
        {
            var receivers = string.Join(", ", emitter.Select(receiver => receiver.Count));
            LayoutUtility.Label($"{label} [{emitter.Reaction.Count()}] [{receivers}]");
        }

        public static void ShowController(this World world, Controller controller, Dictionary<IRunner, TimeSpan> elapsed, bool details, params string[] path)
        {
            void Node(Node node, IRunner profile = null, IRunner state = null, params string[] current)
            {
                var type = node.Value.GetType();
                var fullLabel = string.IsNullOrWhiteSpace(node.Name) ? type.Format() : $"{type.Format()}.{node.Name}";
                var label = details ? fullLabel : string.IsNullOrWhiteSpace(node.Name) ? type.Format() : node.Name;
                current = current.Append(fullLabel).ToArray();

                void Descend()
                {
                    LayoutUtility.Show(true, type, current);
                    for (var i = 0; i < node.Children.Length; i++)
                        Node(node.Children[i], profile, state, current.Append(i.ToString()).ToArray());
                }

                (bool enabled, bool expanded) Label(Type foldout = null)
                {
                    var span = profile != null && elapsed.TryGetValue(profile, out var value2) ? value2 : (TimeSpan?)null;
                    var enabled = state != null && controller.TryState(state, out var value3) ? value3 == Controller.States.Enabled : (bool?)null;
                    var expanded = foldout != null;

                    using (LayoutUtility.Horizontal())
                    {
                        var format = "({0:0.000} ms)";
                        var milliseconds = span is TimeSpan time ? string.Format(format, time.TotalMilliseconds) : " ";

                        using (LayoutUtility.Disable(enabled is false))
                        {
                            if (foldout == null) LayoutUtility.Label(label);
                            else expanded = LayoutUtility.Foldout(label, foldout, current);
                        }

                        using (LayoutUtility.NoIndent())
                        {
                            EditorGUILayout.SelectableLabel(
                                milliseconds,
                                new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleRight },
                                GUILayout.Width(milliseconds.Length * 8f), GUILayout.Height(EditorGUIUtility.singleLineHeight));

                            using (LayoutUtility.Disable(!EditorApplication.isPlaying))
                            {
                                if (enabled.HasValue)
                                {
                                    EditorGUI.BeginChangeCheck();
                                    enabled = EditorGUILayout.Toggle(GUIContent.none, enabled.Value, GUILayout.Width(16f));
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        if (enabled.Value) controller.Enable(state);
                                        else controller.Disable(state);
                                    }
                                }
                            }
                        }
                    }

                    profile = null;
                    state = null;
                    return (enabled ?? true, expanded);
                }

                void Default(bool show)
                {
                    if (show || details)
                    {
                        var (enabled, expanded) = Label(type);
                        if (expanded)
                        {
                            using (LayoutUtility.Disable(!enabled))
                            using (LayoutUtility.Indent()) Descend();
                        }
                    }
                    else Descend();
                }
                switch (node.Value)
                {
                    case Profile _ when controller.TryRunner(node, out var runner): profile = runner; Default(false); break;
                    case State _ when controller.TryRunner(node, out var runner): state = runner; Default(false); break;
                    case Map _: Descend(); break;
                    case Entia.Nodes.System _ when controller.TryRunner(node, out var runner) && runner.Instance is ISystem system:
                        using (LayoutUtility.Box())
                        {
                            var fields = system.GetType().InstanceFields();
                            var (enabled, expanded) = Label(system.GetType());
                            if (expanded)
                            {
                                using (LayoutUtility.Disable(!enabled))
                                using (LayoutUtility.Indent())
                                {
                                    for (int i = 0; i < fields.Length; i++)
                                    {
                                        var field = fields[i];
                                        var format = $"{field.Name}: {field.FieldType.Format()}";
                                        LayoutUtility.Field(
                                            field,
                                            system,
                                            value => world.ShowValue(
                                                $"{field.Name}: {field.FieldType.Format()}",
                                                value,
                                                current.Append(field.Name, field.FieldType.FullName, i.ToString()).ToArray()),
                                            () => LayoutUtility.IsDisabled(field) || (!field.IsPublic && field.IsInitOnly));
                                    }
                                }
                            }
                        }
                        break;
                    case IWrapper _: Default(false); break;
                    default: Default(true); break;
                }
            }

            Node(controller.Root.node, null, null, path);
        }

        public static void ShowNode(this World world, Node node, bool skip, Dictionary<Node, Result<(IDependency[] dependencies, IRunner runner)>> cache, bool details, params string[] path)
        {
            void Next(Node currentNode, params string[] currentPath)
            {
                var type = currentNode.Value.GetType();
                var fullLabel = string.IsNullOrWhiteSpace(currentNode.Name) ? type.Format() : $"{type.Format()}.{currentNode.Name}";
                var label = details ? fullLabel : string.IsNullOrWhiteSpace(currentNode.Name) ? type.Format() : currentNode.Name;
                var result =
                    cache.TryGetValue(currentNode, out var value) ? value :
                    cache[currentNode] = Result.And(world.Analyze(currentNode, node), world.Build(currentNode, node));
                currentPath = currentPath.Append(fullLabel).ToArray();

                void Descend()
                {
                    if (details && result.TryValue(out var pair))
                    {
                        var phases = pair.runner.Phases()
                            .Distinct()
                            .Select(phase => phase.Format())
                            .OrderBy(_ => _)
                            .ToArray();
                        var dependencies = pair.dependencies
                            .Distinct()
                            .Select(dependency => dependency.ToString())
                            .OrderBy(_ => _)
                            .ToArray();
                        using (LayoutUtility.Disable())
                        {
                            LayoutUtility.ChunksFoldout(
                                nameof(Phases),
                                phases,
                                (phase, _) => LayoutUtility.Label(phase),
                                typeof(IPhase),
                                currentPath.Append(nameof(IPhase)).ToArray());
                            LayoutUtility.ChunksFoldout(
                                nameof(Dependencies),
                                dependencies,
                                (dependency, _) => LayoutUtility.Label(dependency),
                                typeof(IDependency),
                                currentPath.Append(nameof(IDependency)).ToArray());
                        }
                    }

                    for (var i = 0; i < currentNode.Children.Length; i++) Next(currentNode.Children[i], currentPath.Append(i.ToString()).ToArray());
                }

                bool Label(MonoScript script = null, Type foldout = null)
                {
                    var expanded = foldout != null;
                    if (foldout == null)
                    {
                        if (script == null) LayoutUtility.Label(label);
                        else EditorGUILayout.ObjectField(script, typeof(MonoScript), false);
                    }
                    else
                    {
                        if (script == null) expanded = LayoutUtility.Foldout(label, foldout, currentPath);
                        else expanded = LayoutUtility.PingFoldout(label, script, foldout, currentPath);
                    }
                    return expanded;
                }

                switch (currentNode.Value)
                {
                    case IWrapper _ when details:
                        using (LayoutUtility.Disable()) Label();
                        using (LayoutUtility.Indent()) Descend();
                        break;
                    case IWrapper _: Descend(); break;
                    case INode _ when skip && !details: skip = false; Descend(); break;
                    case Entia.Nodes.System system:
                        using (LayoutUtility.Box())
                        {
                            ReferenceUtility.TryFindScript(system.Type, out var script);
                            if (Label(script, system.Type))
                            {
                                using (LayoutUtility.Indent())
                                {
                                    var fields = system.Type.GetFields(TypeUtility.Instance);
                                    Descend();

                                    if (details)
                                    {
                                        foreach (var field in fields)
                                        {
                                            var dependencies = world.Dependencies(field)
                                                .Select(dependency => dependency.ToString())
                                                .OrderBy(_ => _)
                                                .ToArray();
                                            LayoutUtility.ChunksFoldout(
                                                $"{field.Name}: {field.FieldType.Format()}",
                                                dependencies,
                                                (dependency, _) => { using (LayoutUtility.Disable()) LayoutUtility.Label(dependency); },
                                                typeof(IDependency),
                                                currentPath.Append(field.Name, nameof(IDependency)).ToArray());
                                        }
                                    }
                                    else
                                    {
                                        foreach (var field in fields)
                                            LayoutUtility.Label($"{field.Name}: {field.FieldType.Format()}");
                                    }
                                }
                            }
                        }
                        break;
                    case Parallel _ when result.TryMessages(out var messages):
                        Label();
                        using (LayoutUtility.Indent())
                        {
                            messages.Distinct().Iterate(message => LayoutUtility.ErrorLabel($"<i>{message}</i>"));
                            Descend();
                        }
                        break;
                    default:
                        Label();
                        using (LayoutUtility.Indent()) Descend();
                        break;
                }
            }

            Next(node, path);
        }

        public static void ShowReceiver(this World _, string label, IReceiver receiver) =>
            LayoutUtility.Label($"{label} ({receiver.Count})");

        public static void ShowReaction(this World _, string label, IReaction reaction) =>
            LayoutUtility.Label($"{label} ({reaction.Count()})");
    }
}
