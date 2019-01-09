using Entia.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [CustomPropertyDrawer(typeof(TypeEnumAttribute))]
    public sealed class TypeEnumDrawer : PropertyDrawer
    {
        static readonly Dictionary<TypeEnumAttribute, Type[]> _filterToTypes = new Dictionary<TypeEnumAttribute, Type[]>();

        public TypeEnumAttribute Attribute => attribute as TypeEnumAttribute;
        public Type[] Types =>
            _filterToTypes.TryGetValue(Attribute, out var types) ? types :
            _filterToTypes[Attribute] = TypeUtility.AllTypes().Where(Valid).OrderBy(type => type.FullName).ToArray();

        bool Valid(Type type) => !type.IsInterface &&
            Valid(TypeEnumAttribute.Filters.IsAbstract, TypeEnumAttribute.Filters.IsNotAbstract, type.IsAbstract) &&
            Valid(TypeEnumAttribute.Filters.IsClass, TypeEnumAttribute.Filters.IsNotClass, type.IsClass) &&
            Valid(TypeEnumAttribute.Filters.IsInterface, TypeEnumAttribute.Filters.IsNotInterface, type.IsInterface) &&
            Valid(TypeEnumAttribute.Filters.IsStruct, TypeEnumAttribute.Filters.IsNotStruct, type.IsValueType) &&
            Valid(TypeEnumAttribute.Filters.IsGeneric, TypeEnumAttribute.Filters.IsNotGeneric, type.IsGenericType) &&
            Implements(type) &&
            Matches(type);
        bool Implements(Type type) => Attribute.Implementations == null || Attribute.Implementations.All(filter => type.Is(filter, true, true));
        bool Matches(Type type) => string.IsNullOrEmpty(Attribute.Regex) || Regex.IsMatch(type.FullName, Attribute.Regex);
        bool Valid(TypeEnumAttribute.Filters @is, TypeEnumAttribute.Filters not, bool value) => Valid(@is, value) && Valid(not, !value);
        bool Valid(TypeEnumAttribute.Filters filter, bool value) => !Has(filter) || value;
        bool Has(TypeEnumAttribute.Filters filter) => (Attribute.Filter & ~filter) != Attribute.Filter;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                EditorGUI.BeginProperty(position, label, property);

                var names = Types.Select(type => type.AssemblyQualifiedName).ToArray();
                var contents = Types.Select(type => new GUIContent(type.FullFormat())).ToArray();
                var index = Array.IndexOf(names, property.stringValue);
                if (index < 0 && Attribute.Default is Type @default) index = Array.IndexOf(names, @default.AssemblyQualifiedName);
                if (index < 0) index = 0;

                EditorGUI.BeginChangeCheck();
                index = EditorGUI.Popup(position, label, index, contents);
                if (EditorGUI.EndChangeCheck() && index >= 0 && index < names.Length)
                {
                    property.stringValue = names[index];
                    property.serializedObject.ApplyModifiedProperties();
                }

                EditorGUI.EndProperty();
            }
            else EditorGUI.LabelField(position, label, "Field must be of type 'string'.");
        }
    }
}
