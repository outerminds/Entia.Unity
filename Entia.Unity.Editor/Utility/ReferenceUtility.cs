using Entia.Core;
using Entia.Messages;
using Entia.Modules;
using Entia.Unity.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace Entia.Unity.Editor
{
    [InitializeOnLoad]
    public static class ReferenceUtility
    {
        static class Cache<T> where T : struct, IMessage
        {
            public static readonly Dictionary<Type, Action<SerializedObject, IReference>> Emits = new Dictionary<Type, Action<SerializedObject, IReference>>();

            public static void Emit(string name, SerializedObject serialized, IReference reference)
            {
                if (reference?.World is World)
                {
                    var type = reference.GetType();
                    if (Emits.TryGetValue(type, out var emit)) emit(serialized, reference);
                    Emits[type] = emit = typeof(ReferenceUtility)
                        .GetMethods(TypeUtility.Static)
                        .First(method => method.Name == name && method.IsGenericMethod)
                        .MakeGenericMethod(type)
                        .CreateDelegate<Action<SerializedObject, IReference>>();
                    emit(serialized, reference);
                }
            }
        }

        static readonly Lazy<Dictionary<Type, string>> _typeToLink = new Lazy<Dictionary<Type, string>>(() => TypeUtility.AllTypes
            .AsParallel()
            .SelectMany(type => type.GetCustomAttributes(typeof(GeneratedAttribute), true)
                .OfType<GeneratedAttribute>()
                .Select(attribute => (attribute.Type, attribute.Link))
                .Where(pair => pair.Type != null && !string.IsNullOrWhiteSpace(pair.Link))
                .SelectMany(pair => new[] { (Type: type, pair.Link), pair }))
            .DistinctBy(pair => pair.Type)
            .ToDictionary(pair => pair.Type, pair => pair.Link));

        static ReferenceUtility() => EditorUtility.Delayed(Update);

        public static SerializedProperty Script(SerializedObject serialized)
        {
            var iterator = serialized.GetIterator();
            iterator.Next(true);
            iterator.NextVisible(false);

            using (LayoutUtility.Disable())
            {
                if (TryFindScript(serialized.targetObject.GetType(), out var script))
                    EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), false);
                else
                    EditorGUILayout.PropertyField(iterator);
            }

            return iterator;
        }

        public static bool TryFindScript(Type type, out MonoScript script)
        {
            if (_typeToLink.Value.TryGetValue(type, out var link))
            {
                script = AssetDatabase.LoadAssetAtPath<MonoScript>(link);
                return script != null;
            }

            script = default;
            return false;
        }

        public static Disposable World(SerializedObject serialized, IWorldReference reference, ref ReorderableList modifiers)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            using (LayoutUtility.Disable(EditorApplication.isPlayingOrWillChangePlaymode))
                LayoutUtility.ScriptableList<WorldModifier>(serialized.FindProperty("_modifiers"), ref modifiers, TemplateUtility.CreateModifier);
            EditorGUILayout.Separator();

            var apply = LayoutUtility.Apply(serialized);
            return new Disposable(() =>
            {
                OnInspector(serialized, reference);
                apply.Dispose();
                if (EditorGUI.EndChangeCheck()) { Update(); OnValidate(reference); }
            });
        }

        public static Disposable Controller(SerializedObject serialized, IControllerReference reference, ref ReorderableList nodes)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            using (LayoutUtility.Disable(EditorApplication.isPlayingOrWillChangePlaymode))
                LayoutUtility.ScriptableList<NodeReference>(serialized.FindProperty("_nodes"), ref nodes, TemplateUtility.CreateNode);
            EditorGUILayout.Separator();

            var apply = LayoutUtility.Apply(serialized);
            return new Disposable(() =>
            {
                OnInspector(serialized, reference);
                apply.Dispose();
                if (EditorGUI.EndChangeCheck()) { Update(); OnValidate(reference); }
            });
        }

        public static Disposable Entity(SerializedObject serialized, IEntityReference reference)
        {
            if (reference.Entity && reference.World is World world)
            {
                reference.PostInitialize();
                EditorGUI.BeginChangeCheck();
                world.ShowEntity(reference.Entity.ToString(world), reference.Entity, nameof(EntityReferenceEditor), reference.Entity.ToString());

                var apply = LayoutUtility.Apply(serialized);
                return new Disposable(() =>
                {
                    OnInspector(serialized, reference);
                    apply.Dispose();
                    if (EditorGUI.EndChangeCheck()) OnValidate(reference);
                });
            }
            return Disposable.Empty;
        }

        public static Disposable Component<T>(SerializedObject serialized, IEnumerable<T> references, out SerializedProperty property) where T : IComponentReference
        {
            var instances = references.Where(reference => reference.Entity && reference.World is World).ToArray();
            foreach (var instance in instances) instance.Raw = instance.Value;
            EditorGUI.BeginChangeCheck();

            var first = references.First();
            using (LayoutUtility.Horizontal())
            {
                property = Script(serialized);
                using (LayoutUtility.Disable(!EditorApplication.isPlaying))
                {
                    if (first.World is World world && first.Entity)
                    {
                        if (world.Components().Has(first.Entity, first.Type))
                        {
                            if (LayoutUtility.MinusButton())
                                foreach (var reference in references) reference.World?.Components().Remove(reference.Entity, reference.Type);
                        }
                        else
                        {
                            if (LayoutUtility.PlusButton())
                                foreach (var reference in references) reference.World?.Components().Set(reference.Entity, reference.Raw);
                        }
                    }
                }
            }

            var disable = LayoutUtility.Disable(first.Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            var apply = LayoutUtility.Apply(serialized);
            return new Disposable(() =>
            {
                OnInspector(serialized, first);
                apply.Dispose();
                disable.Dispose();
                if (EditorGUI.EndChangeCheck())
                    foreach (var instance in instances) { instance.Value = instance.Raw; OnValidate(instance); }
            });
        }

        public static Disposable Resource<T>(SerializedObject serialized, IEnumerable<T> references, out SerializedProperty property) where T : IResourceReference
        {
            var instances = references.Where(reference => reference.World is World).ToArray();
            foreach (var instance in instances) instance.Raw = instance.Value;
            EditorGUI.BeginChangeCheck();

            var first = references.First();
            property = Script(serialized);
            var disable = LayoutUtility.Disable(first.Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            var apply = LayoutUtility.Apply(serialized);
            return new Disposable(() =>
            {
                OnInspector(serialized, first);
                apply.Dispose();
                disable.Dispose();
                if (EditorGUI.EndChangeCheck())
                    foreach (var instance in instances) { instance.Value = instance.Raw; OnValidate(instance); }
            });
        }

        public static void Update()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            foreach (var reference in UnityEngine.Object.FindObjectsOfType<WorldReference>())
                reference.Initialize();
        }

        static void OnInspector(SerializedObject serialized, IReference reference) =>
            Cache<OnInspector>.Emit(nameof(OnInspector), serialized, reference);
        static void OnInspector<T>(SerializedObject serialized, IReference reference) where T : IReference
        {
            var messages = reference.World.Messages();
            messages.Emit(new OnInspector<T> { Reference = (T)reference, Serialized = serialized });
            messages.Emit(new OnInspector { Reference = reference, Serialized = serialized });
        }
        static void OnValidate(IReference reference) =>
            Cache<OnValidate>.Emit(nameof(OnValidate), default, reference);
        static void OnValidate<T>(SerializedObject _, IReference reference) where T : IReference
        {
            var messages = reference.World.Messages();
            messages.Emit(new OnValidate<T> { Reference = (T)reference });
            messages.Emit(new OnValidate { Reference = reference });
        }
    }
}