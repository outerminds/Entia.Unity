using Entia.Core;
using Entia.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class ReferenceUtility
    {
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
            var path = type.GetCustomAttributes(true)
                .OfType<GeneratedAttribute>()
                .Select(attribute => attribute.Link)
                .FirstOrDefault(link => !string.IsNullOrWhiteSpace(link));

            script = string.IsNullOrWhiteSpace(path) ? default : AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            return script != null;
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
