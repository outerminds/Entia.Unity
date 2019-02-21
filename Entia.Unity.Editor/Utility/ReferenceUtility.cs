using Entia.Core;
using Entia.Modules;
using Entia.Unity.Generation;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

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

        public static Disposable Component<T>(SerializedObject serialized, IEnumerable<T> references, out SerializedProperty property) where T : IComponentReference
        {
            if (EditorApplication.isPlaying)
            {
                foreach (var reference in references) reference.Raw = reference.Value;
                EditorGUI.BeginChangeCheck();
            }

            var first = references.First();
            using (LayoutUtility.Horizontal())
            {
                property = Script(serialized);
                if (first.World is World world && first.Entity)
                {
                    if (world.Components().Has(first.Entity, first.Type))
                    {
                        if (LayoutUtility.MinusButton()) world.Components().Remove(first.Entity, first.Type);
                    }
                    else
                    {
                        if (LayoutUtility.PlusButton()) world.Components().Set(first.Entity, first.Raw);
                    }
                }
            }

            var disable = LayoutUtility.Disable(first.Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            var apply = LayoutUtility.Apply(serialized);
            return new Disposable(() =>
            {
                apply.Dispose();
                disable.Dispose();
                if (EditorApplication.isPlaying && EditorGUI.EndChangeCheck())
                    foreach (var reference in references) reference.Value = reference.Raw;
            });
        }

        public static Disposable Resource<T>(SerializedObject serialized, IEnumerable<T> references, out SerializedProperty property) where T : IResourceReference
        {
            if (EditorApplication.isPlaying)
            {
                foreach (var reference in references) reference.Raw = reference.Value;
                EditorGUI.BeginChangeCheck();
            }

            var first = references.First();
            property = Script(serialized);
            var disable = LayoutUtility.Disable(first.Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            var apply = LayoutUtility.Apply(serialized);

            return new Disposable(() =>
            {
                apply.Dispose();
                disable.Dispose();
                if (EditorApplication.isPlaying && EditorGUI.EndChangeCheck())
                    foreach (var reference in references) reference.Value = reference.Raw;
            });
        }
    }
}
