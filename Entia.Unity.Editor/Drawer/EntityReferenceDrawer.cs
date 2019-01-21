using Entia.Core;
using Entia.Queryables;
using Entia.Mappers;
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
                var types = reference.GetComponents<IComponentReference>().Select(component => component.Type)
                    .Concat(reference.GetComponents<Component>().SelectMany(component => new[] { component.GetType(), component.Map<ToComponent, IComponent>(new ToComponent()).GetType() }))
                    .ToArray();
                var attributes = fieldInfo.GetCustomAttributes(false);
                var missing = attributes
                    .OfType<AllAttribute>()
                    .SelectMany(all => all.Types.Select(metadata => metadata.Type))
                    .Except(types)
                    .Select(type => $"Missing component of type '{type.FullFormat()}'.");
                var either = attributes
                    .OfType<AnyAttribute>()
                    .Select(any => any.Types.Select(metadata => metadata.Type))
                    .Select(any => any.Intersect(types).Any() ? null : $"Missing at least one component from types '{string.Join(", ", any.Select(type => type.FullFormat()))}'.")
                    .Some();
                var exceeding = attributes
                    .OfType<NoneAttribute>()
                    .SelectMany(none => none.Types.Select(metadata => metadata.Type))
                    .Intersect(types)
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
