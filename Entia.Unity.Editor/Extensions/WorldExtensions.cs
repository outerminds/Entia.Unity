using Entia.Core;
using Entia.Dependencies;
using Entia.Injectables;
using Entia.Modules;
using Entia.Modules.Build;
using Entia.Modules.Component;
using Entia.Modules.Control;
using Entia.Modules.Group;
using Entia.Modules.Message;
using Entia.Modules.Query;
using Entia.Nodes;
using Entia.Phases;
using Entia.Systems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public static string Name(this World world, Entity entity)
        {
            ref var debug = ref world.Components().GetOrDummy<Components.Debug>(entity, out var success);
            if (success) return $"{debug.Name}: {entity}";
            return entity.ToString();
        }

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
            var type = injectable.GetType();
            if (injectable is AllEntities || injectable is AllEntities.Read) world.ShowEntities(label, world.Entities(), path);
            else if (injectable is AllComponents) world.ShowComponents(label, world.Entities(), path);
            else if (injectable.Is(typeof(Resource<>), definition: true) || injectable.Is(typeof(Resource<>.Read), definition: true))
            {
                var resource = type.GetGenericArguments()[0];
                world.ShowResource(label, world.Resources().Get(resource), path);
            }
            else if (injectable.Is(typeof(Components<>), definition: true) || injectable.Is(typeof(Components<>.Read), definition: true))
            {
                var component = type.GetGenericArguments()[0];
                var pairs = world.Entities()
                    .Select(entity => world.Components().TryGet(entity, component, out var current) ? (entity, component: current) : default)
                    .Where(pair => pair.component != null);
                world.ShowComponents(label, pairs, component, path);
            }
            else if (injectable.Is(typeof(Injectables.Emitter<>), definition: true) && injectable.GetValue<IEmitter>("_emitter").TryValue(out var emitter))
                world.ShowEmitter(label, emitter);
            else if (injectable.Is(typeof(Injectables.Receiver<>), definition: true) && injectable.GetValue<IReceiver>("_receiver").TryValue(out var receiver))
                world.ShowReceiver(label, receiver);
            else if (injectable.Is(typeof(Injectables.Reaction<>), definition: true) && injectable.GetValue<IReaction>("_reaction").TryValue(out var reaction))
                world.ShowReaction(label, reaction);
            else if (injectable.GetValue<IGroup>("_group").TryValue(out var group))
                world.ShowGroup(label, group, path);
            else LayoutUtility.Label(label);
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
                    foldout: data =>
                    {
                        using (LayoutUtility.Horizontal())
                        {
                            var expanded = LayoutUtility.Foldout(data.label, data.type, data.path);
                            if (LayoutUtility.PlusButton())
                            {
                                var menu = new GenericMenu();
                                foreach (var type in Concretes)
                                {
                                    var content = new GUIContent(string.Join("/", type.Path().SkipLast().Append(type.Format())));
                                    if (world.Components().Has(entity, type)) menu.AddDisabledItem(content, false);
                                    else menu.AddItem(content, false, () => world.Components().Set(entity, TypeUtility.GetDefault(type) as IComponent));
                                }
                                menu.ShowAsContext();
                            }

                            if (LayoutUtility.MinusButton()) world.Entities().Destroy(entity);
                            return expanded;
                        }
                    });
            }
        }

        public static void ShowEntities(this World world, string label, IEnumerable<Entity> entities, params string[] path) =>
            LayoutUtility.ChunksFoldout(
                label,
                entities.ToArray(),
                (entity, index) => world.ShowEntity(world.Name(entity), entity, path.Append(index.ToString()).ToArray()),
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
                (pair, index2) => world.ShowComponent(world.Name(pair.entity), pair.entity, pair.component, path.Append(index2.ToString()).ToArray()),
                type,
                path.Append(type.FullName).ToArray());

        public static void ShowComponent(this World world, string label, Entity entity, IComponent component, params string[] path)
        {
            var type = component.GetType();

            using (LayoutUtility.Horizontal())
            {
                using (LayoutUtility.Vertical())
                using (LayoutUtility.Disable(type.GetCustomAttributes(true).OfType<DisableAttribute>().Any()))
                {
                    EditorGUI.BeginChangeCheck();
                    component = LayoutUtility.Object(label, component, type, path.Append(type.FullName, entity.ToString()).ToArray()) as IComponent;
                    if (EditorGUI.EndChangeCheck() && component is IComponent current) world.Components().Set(entity, current);
                }

                if (LayoutUtility.MinusButton())
                    world.Components().Remove(entity, type);
            }
        }

        public static void ShowGroup(this World world, string label, IGroup group, params string[] path) =>
            world.ShowEntities(label, group.Entities, path);

        public static void ShowResource(this World world, string label, IResource resource, params string[] path)
        {
            var type = resource.GetType();
            EditorGUI.BeginChangeCheck();
            resource = LayoutUtility.Object(label, resource, type, path) as IResource;
            if (EditorGUI.EndChangeCheck() && resource is IResource current) world.Resources().Set(current);
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
                resources.GetType(),
                path);

        public static void ShowEmitter(this World _, string label, IEmitter emitter)
        {
            var receivers = string.Join(", ", emitter.Receivers.Select(receiver => receiver.Count));
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
                            var fields = TypeUtility.GetFields(system.GetType());
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

        public static void ShowNode(this World world, Node node, Dictionary<Node, Result<(IDependency[] dependencies, IRunner runner)>> cache, bool details, params string[] path)
        {
            void Next(Node currentNode, params string[] currentPath)
            {
                var type = currentNode.Value.GetType();
                var fullLabel = string.IsNullOrWhiteSpace(currentNode.Name) ? type.Format() : $"{type.Format()}.{currentNode.Name}";
                var label = details ? fullLabel : string.IsNullOrWhiteSpace(currentNode.Name) ? type.Format() : currentNode.Name;
                var result =
                    cache.TryGetValue(currentNode, out var value) ? value :
                    cache[currentNode] = Result.And(world.Analyzers().Analyze(currentNode, node), world.Builders().Build(currentNode, node));
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
                        else
                        {
                            var area = EditorGUI.IndentedRect(EditorGUILayout.BeginVertical());
                            {
                                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && area.Contains(Event.current.mousePosition))
                                {
                                    if (Event.current.clickCount == 1) EditorGUIUtility.PingObject(script);
                                    else if (Event.current.clickCount == 2)
                                    {
                                        AssetDatabase.OpenAsset(script);
                                        GUIUtility.ExitGUI();
                                        Event.current.Use();
                                    }
                                }

                                var content = EditorGUIUtility.ObjectContent(script, typeof(MonoScript));
                                content.text = label;
                                using (LayoutUtility.IconSize(new Vector2(12f, 12f))) expanded = LayoutUtility.Foldout(content, foldout, currentPath);
                            }
                            EditorGUILayout.EndVertical();
                        }
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
                                            var dependencies = world.Dependers()
                                                .Dependencies(field)
                                                .Distinct()
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
