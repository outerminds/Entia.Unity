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

        public static Disposable Component<T>(IEnumerable<T> references) where T : IComponentReference
        {
            foreach (var reference in references) reference.Raw = reference.Component;
            var disable = LayoutUtility.Disable(references.First().Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            EditorGUI.BeginChangeCheck();

            return new Disposable(() =>
            {
                disable.Dispose();
                if (EditorGUI.EndChangeCheck()) foreach (var reference in references) reference.Component = reference.Raw;
            });
        }

        public static Disposable Resource<T>(IEnumerable<T> references) where T : IResourceReference
        {
            foreach (var reference in references) reference.Raw = reference.Resource;
            var disable = LayoutUtility.Disable(references.First().Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            EditorGUI.BeginChangeCheck();

            return new Disposable(() =>
            {
                disable.Dispose();
                if (EditorGUI.EndChangeCheck()) foreach (var reference in references) reference.Resource = reference.Raw;
            });
        }
    }
}
