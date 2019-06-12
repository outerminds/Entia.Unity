using Entia.Core;
using Entia.Modules;
using Entia.Unity.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Profiling;

namespace Entia.Unity.Editor
{
    public static class ReferenceUtility
    {
        static readonly Dictionary<Type, string> _typeToLink = TypeUtility.AllTypes
            .AsParallel()
            .SelectMany(type => type.GetCustomAttributes(typeof(GeneratedAttribute), true)
                .OfType<GeneratedAttribute>()
                .Select(attribute => (attribute.Type, attribute.Link))
                .Where(pair => pair.Type != null && !string.IsNullOrWhiteSpace(pair.Link))
                .SelectMany(pair => new[] { (Type: type, pair.Link), pair }))
            .DistinctBy(pair => pair.Type)
            .ToDictionary(pair => pair.Type, pair => pair.Link);

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
            if (_typeToLink.TryGetValue(type, out var link))
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
            return new Disposable(() => { if (EditorGUI.EndChangeCheck()) Update(); });
        }

        public static Disposable Controller(SerializedObject serialized, IControllerReference reference, ref ReorderableList nodes)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.Space();
            using (LayoutUtility.Disable(EditorApplication.isPlayingOrWillChangePlaymode))
                LayoutUtility.ScriptableList<NodeReference>(serialized.FindProperty("_nodes"), ref nodes, TemplateUtility.CreateNode);
            EditorGUILayout.Separator();
            return new Disposable(() => { if (EditorGUI.EndChangeCheck()) Update(); });
        }

        public static Disposable Entity(IEntityReference reference)
        {
            if (reference.Entity && reference.World is World world)
            {
                reference.PostInitialize();
                world.ShowEntity(reference.Entity.ToString(world), reference.Entity, nameof(EntityReferenceEditor), reference.Entity.ToString());
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
                apply.Dispose();
                disable.Dispose();
                if (EditorGUI.EndChangeCheck()) foreach (var instance in instances) instance.Value = instance.Raw;
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
                apply.Dispose();
                disable.Dispose();
                if (EditorGUI.EndChangeCheck()) foreach (var instance in instances) instance.Value = instance.Raw;
            });
        }

        public static void Update()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            foreach (var reference in UnityEngine.Object.FindObjectsOfType<WorldReference>())
                reference.Initialize();
        }
    }
}