using Entia.Core;
using Entia.Queryables;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [CustomPropertyDrawer(typeof(EntityReference))]
    public sealed class EntityReferenceDrawer : PropertyDrawer
    {
        string Errors(SerializedProperty property)
        {
            if (property.objectReferenceValue is EntityReference reference)
            {
                var types = reference.GetComponents<Component>()
                    .SelectMany(component =>
                        component is IComponentReference casted ? new[] { casted.Type } :
                        ComponentDelegate.TryGet(component.GetType(), out var modifier) ? new[] { component.GetType(), modifier.Type } :
                        new[] { component.GetType() })
                    .ToArray();
                var attributes = fieldInfo.GetCustomAttributes(false);
                var missing = attributes
                    .OfType<AllAttribute>()
                    .SelectMany(all => all.Components)
                    .Where(component => types.None(type => type.Is(component, true, true)))
                    .Select(component => $"Missing component of type '{component.FullFormat()}'.");
                var either = attributes
                    .OfType<AnyAttribute>()
                    .Select(any => any.Components)
                    .Where(components => components.None(component => types.Any(type => type.Is(component, true, true))))
                    .Select(components => $"Missing at least one component from types '{string.Join(", ", components.Select(type => type.FullFormat()))}'.");
                var exceeding = attributes
                    .OfType<NoneAttribute>()
                    .SelectMany(none => none.Components)
                    .Where(component => types.Any(type => type.Is(component, true, true)))
                    .Select(type => $"Exceeding component of type '{type.FullFormat()}'.");
                return string.Join(Environment.NewLine, missing.Concat(either).Concat(exceeding));
            }

            return null;
        }

        Vector2 Size(Rect position, string errors)
        {
            var size = EditorStyles.helpBox.CalcSize(new GUIContent(errors, LayoutUtility.ErrorIcon));
            size.x = Mathf.Max(size.x, position.width);
            size.y = Mathf.Max(size.y, 32f);
            return size;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (Errors(property))
            {
                case string errors when !string.IsNullOrWhiteSpace(errors):
                    var size = Size(position, errors);
                    var height = EditorGUI.GetPropertyHeight(property, label);
                    label.tooltip = errors;

                    EditorGUI.HelpBox(new Rect(position) { size = size }, errors, MessageType.Error);
                    EditorGUI.PropertyField(new Rect(position) { y = position.y + size.y, height = height }, property, label, true);
                    break;
                default: EditorGUI.PropertyField(position, property, label, true); break;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            switch (Errors(property))
            {
                case string errors when !string.IsNullOrWhiteSpace(errors):
                    return Size(default, errors).y + EditorGUI.GetPropertyHeight(property, label);
                default: return EditorGUI.GetPropertyHeight(property, label);
            }
        }
    }
}
