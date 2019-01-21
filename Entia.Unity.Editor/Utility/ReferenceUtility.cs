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
            var disable = LayoutUtility.Disable(references.First().Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            EditorGUI.BeginChangeCheck();

            return new Disposable(() =>
            {
                disable.Dispose();
                var changed = EditorGUI.EndChangeCheck();

                foreach (var reference in references)
                {
                    var world = reference.World;
                    if (world == null) return;

                    if (world.Components().TryGet(reference.Entity, reference.Type, out var component))
                    {
                        if (changed) world.Components().Set(reference.Entity, component = reference.Component);
                        if (EditorApplication.isPlaying) reference.Component = component;
                    }
                }
            });
        }

        public static Disposable Resource<T>(IEnumerable<T> references) where T : IResourceReference
        {
            var disable = LayoutUtility.Disable(references.First().Type.GetCustomAttributes(false).OfType<DisableAttribute>().Any());
            EditorGUI.BeginChangeCheck();

            return new Disposable(() =>
            {
                disable.Dispose();
                var changed = EditorGUI.EndChangeCheck();

                foreach (var reference in references)
                {
                    var world = reference.World;
                    if (world == null) return;

                    if (changed) world.Resources().Set(reference.Resource);
                    if (EditorApplication.isPlaying) reference.Resource = world.Resources().Get(reference.Type);
                }
            });
        }
    }
}
