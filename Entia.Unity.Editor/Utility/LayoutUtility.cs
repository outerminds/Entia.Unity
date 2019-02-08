﻿using Entia.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class LayoutUtility
    {
        class Comparer : IEqualityComparer<(Type type, string[] path)>
        {
            public bool Equals((Type type, string[] path) x, (Type type, string[] path) y) => x.type == y.type && x.path.SequenceEqual(y.path);
            public int GetHashCode((Type type, string[] path) obj)
            {
                var hash = obj.type.GetHashCode();
                foreach (var segment in obj.path) hash ^= segment.GetHashCode();
                return hash;
            }
        }

        public static GUIStyle BoldFoldout => new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
        public static GUIStyle ItalicFoldout => new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Italic };
        public static GUIStyle MiniLabel => new GUIStyle(EditorStyles.miniLabel);
        public static GUIStyle BoldLabel => new GUIStyle(EditorStyles.boldLabel);
        public static GUIStyle BoldToggle => new GUIStyle(EditorStyles.toggle) { fontStyle = FontStyle.Bold };
        public static GUIStyle ItalicLabel => new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Italic };
        public static readonly Texture2D ErrorIcon = EditorGUIUtility.Load("icons/console.erroricon.png") as Texture2D;

        static readonly Dictionary<(Type type, string[] path), bool> _show = new Dictionary<(Type type, string[] path), bool>(new Comparer());

        public static Disposable Apply(params SerializedObject[] serialized)
        {
            serialized.Iterate(current => current.Update());
            return new Disposable(() => serialized.Iterate(current => current.ApplyModifiedProperties()));
        }

        public static Disposable Box()
        {
            var area = EditorGUI.IndentedRect(EditorGUILayout.BeginVertical());
            area.height++;
            GUI.Box(area, "");
            return new Disposable(EditorGUILayout.EndVertical);
        }

        public static Disposable Indent()
        {
            EditorGUI.indentLevel++;
            return new Disposable(() => EditorGUI.indentLevel--);
        }

        public static Disposable Horizontal(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(options);
            return new Disposable(EditorGUILayout.EndHorizontal);
        }

        public static Disposable Vertical(params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(options);
            return new Disposable(EditorGUILayout.EndVertical);
        }

        public static Disposable NoIndent()
        {
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            return new Disposable(() => EditorGUI.indentLevel = indent);
        }

        public static Disposable IconSize(Vector2 size)
        {
            var old = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(size);
            return new Disposable(() => EditorGUIUtility.SetIconSize(old));
        }

        public static Disposable LabelWidth(float width)
        {
            var old = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = width;
            return new Disposable(() => EditorGUIUtility.labelWidth = old);
        }

        public static Disposable Disable(bool disabled = true)
        {
            EditorGUI.BeginDisabledGroup(disabled);
            return new Disposable(EditorGUI.EndDisabledGroup);
        }

        public static Disposable BackgroundColor(Color color)
        {
            var background = GUI.backgroundColor;
            GUI.color = color;
            return new Disposable(() => GUI.color = background);
        }

        public static bool PlusButton() => GUILayout.Button(
            "+",
            new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                fontStyle = FontStyle.Bold,
                clipping = TextClipping.Overflow,
            },
            GUILayout.Height(16),
            GUILayout.Width(16));

        public static bool MinusButton() => GUILayout.Button(
            "-",
            new GUIStyle(GUI.skin.button)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                clipping = TextClipping.Overflow
            },
            GUILayout.Height(16),
            GUILayout.Width(16));

        public static bool Toggle(string label, bool value)
        {
            using (LayoutUtility.NoIndent())
            {
                var content = new GUIContent(label);
                var style = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter };
                value = EditorGUILayout.Toggle(GUIContent.none, value, EditorStyles.miniButton, GUILayout.Width(style.CalcSize(content).x + 10));
                var area = GUILayoutUtility.GetLastRect();
                EditorGUI.LabelField(area, content, style);
            }
            return value;
        }

        public static void Label(string label, GUIStyle style = null)
        {
            var option = GUILayout.Height(EditorGUIUtility.singleLineHeight);
            style = style ?? new GUIStyle(EditorStyles.label);
            style.richText = true;
            EditorGUILayout.SelectableLabel(label, style, option);
        }

        public static void ErrorLabel(string label, GUIStyle style = null)
        {
            var area = EditorGUILayout.BeginHorizontal();
            using (Indent()) Label(label, style);
            EditorGUILayout.EndHorizontal();
            area.x -= 3f;
            area.y--;
            area.width = Math.Max(area.width, 20f);
            area.height = Math.Max(area.height, 20f);
            EditorGUI.LabelField(area, new GUIContent("", ErrorIcon));
        }

        public static void Show(bool value, Type type, params string[] path) => _show[(type, path)] = value;

        public static bool Foldout(string label, Type type, IEnumerable<string> path) => Foldout(label, type, path.ToArray());
        public static bool Foldout(string label, Type type, params string[] path) => Foldout(label, null, type, path);
        public static bool Foldout(string label, GUIStyle style, Type type, params string[] path) => Foldout(new GUIContent(label), style, type, path);
        public static bool Foldout(GUIContent label, Type type, params string[] path) => Foldout(label, null, type, path);
        public static bool Foldout(GUIContent label, GUIStyle style, Type type, params string[] path)
        {
            var key = (type, path);
            var show = _show.TryGetValue(key, out var value) && value;
            return _show[key] = style == null ?
                EditorGUILayout.Foldout(show, label) :
                EditorGUILayout.Foldout(show, label, style);
        }

        public static bool ChunksFoldout<T>(string label, T[] items, Action<T, int> each, Type type = null, string[] path = null, GUIStyle style = null, Func<(string label, Type type, string[] path), bool> foldout = null)
        {
            type = type ?? typeof(T);
            path = path ?? new string[0];
            foldout = foldout ?? (data => Foldout(data.label, style, data.type, data.path));

            if (foldout(($"{label} ({items.Length})", type, path)))
            {
                using (Indent()) Chunks(items, each, type, path);
                return true;
            }

            return false;
        }

        public static void Chunks<T>(T[] items, Action<T, int> each, Type type, params string[] path) =>
            Chunks(items, 50, each, type, path);
        public static void Chunks<T>(T[] items, int size, Action<T, int> each, Type type, params string[] path)
        {
            (Type type, string[] path) Key(int index) =>
                (type, path: path.Append(index.ToString(), size.ToString()).ToArray());

            for (var i = 0; i < items.Length; i += size)
            {
                var key = Key(i);
                var show = _show.TryGetValue(key, out var value) ? value : i == 0;

                if (show)
                    for (int j = 0, k = i; j < size && k < items.Length; j++, k++) each(items[k], k);
                else if (Foldout($"[{i} .. {Math.Min(i + size, items.Length) - 1}]", ItalicFoldout, key.type, key.path))
                    for (var j = 0; j < items.Length; j += size) _show[Key(j)] = j == i;
            }
        }

        public static void Field(FieldInfo field, object instance, params string[] path) =>
            Field(field, instance, value => Object(field.Name, value, field.FieldType, path.Append(field.Name).ToArray()));

        public static void Field(FieldInfo field, object instance, Func<object, object> show)
        {
            using (Disable(field.IsInitOnly || field.GetCustomAttributes<DisableAttribute>().Any() || field.DeclaringType.GetCustomAttributes<DisableAttribute>().Any()))
            {
                if (Result.Try(() => field.GetValue(instance)).TryValue(out var value))
                {
                    EditorGUI.BeginChangeCheck();
                    value = show(value);
                    if (EditorGUI.EndChangeCheck()) field.SetValue(instance, value);
                }
                else Label($"{field.Name}: {field.FieldType.Format()}");
            }
        }

        public static object Object(string label, object value, Type type, params string[] path)
        {
            switch (value)
            {
                case bool boolean: return EditorGUILayout.Toggle(label, boolean);
                case byte @byte: return (byte)EditorGUILayout.IntField(label, @byte);
                case sbyte @sbyte: return (sbyte)EditorGUILayout.IntField(label, @sbyte);
                case short @short: return (short)EditorGUILayout.IntField(label, @short);
                case ushort @ushort: return (ushort)EditorGUILayout.IntField(label, @ushort);
                case int @int: return EditorGUILayout.IntField(label, @int);
                case uint @uint: return (uint)EditorGUILayout.LongField(label, @uint);
                case long @long: return EditorGUILayout.LongField(label, @long);
                case ulong @ulong: return (ulong)EditorGUILayout.LongField(label, (long)@ulong);
                case float @float: return EditorGUILayout.FloatField(label, @float);
                case double @double: return EditorGUILayout.DoubleField(label, @double);
                case decimal @decimal: return (decimal)EditorGUILayout.DoubleField(label, (double)@decimal);
                case string @string: return EditorGUILayout.TextField(label, @string);
                case Enum @enum: return EditorGUILayout.EnumPopup(label, @enum);
                case Vector2 vector2: return EditorGUILayout.Vector2Field(label, vector2);
                case Vector2Int vector2Int: return EditorGUILayout.Vector2IntField(label, vector2Int);
                case Vector3 vector3: return EditorGUILayout.Vector3Field(label, vector3);
                case Vector3Int vector3Int: return EditorGUILayout.Vector3IntField(label, vector3Int);
                case Vector4 vector4: return EditorGUILayout.Vector4Field(label, vector4);
                case Bounds bounds: return EditorGUILayout.BoundsField(label, bounds);
                case BoundsInt boundsInt: return EditorGUILayout.BoundsIntField(label, boundsInt);
                case Rect rect: return EditorGUILayout.RectField(label, rect);
                case RectInt rectInt: return EditorGUILayout.RectIntField(label, rectInt);
                case Color color: return EditorGUILayout.ColorField(label, color);
                case AnimationCurve curve: return EditorGUILayout.CurveField(label, curve);
                case LayerMask layer: return EditorGUILayout.LayerField(label, layer);
                case UnityEngine.Object @object: return EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, true);
                case null: return Null(label, value, type);
                case IList list when list.GetType().TryElement(out var element): return List(label, list, element, path);
                case IDictionary dictionary when dictionary.GetType().TryElement(out var element) &&
                    element.IsGenericType && element.GetGenericTypeDefinition() == typeof(KeyValuePair<,>):
                    var arguments = element.GetGenericArguments();
                    return Dictionary(label, dictionary, (arguments[0], arguments[1]), path);
                case IEnumerable enumerable when enumerable.GetType().TryElement(out var element):
                    {
                        var items = enumerable.Cast<object>().ToList().AsReadOnly();
                        Object(label, items, element, path);
                        return enumerable;
                    }
                default: return Fields(label, value, type, path);
            }
        }

        public static object Fields(string label, object value, Type type, string[] path)
        {
            var fields = TypeUtility.GetFields(value.GetType(), TypeUtility.PublicInstance);

            if (fields.Length == 0) Label(label);
            else if (Foldout(label, type, path))
                using (Indent()) using (Vertical()) foreach (var field in fields) Field(field, value);

            return value;
        }

        public static object Null(string label, object value, Type type)
        {
            if (type.Is<UnityEngine.Object>())
                return EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, type, true);
            else
            {
                EditorGUILayout.LabelField(label, $"null: ({type.Format()})");
                return value;
            }
        }

        public static object List(string label, IList list, Type element, params string[] path)
        {
            if (Foldout($"{label} ({list.Count})", list.GetType(), path))
            {
                using (Indent())
                {
                    using (Disable(list.IsFixedSize))
                    {
                        var count = EditorGUILayout.IntField("Size", list.Count);
                        while (list.Count < count) list.Add(TypeUtility.GetDefault(element));
                        while (list.Count > count && list.Count > 0) list.RemoveAt(list.Count - 1);
                    }

                    using (Disable(list.IsReadOnly))
                    {
                        for (var i = 0; i < list.Count; i++)
                        {
                            var item = Object($"Element {i}", list[i], element, path.Append(i.ToString()).ToArray());
                            if (!list.IsReadOnly) list[i] = item;
                        }
                    }
                }
            }

            return list;
        }

        public static object Dictionary(string label, IDictionary dictionary, (Type key, Type value) element, params string[] path)
        {
            if (Foldout($"{label} ({dictionary.Count})", dictionary.GetType(), path))
            {
                using (Indent())
                {
                    using (Disable()) EditorGUILayout.IntField("Size", dictionary.Count);
                    using (Disable(dictionary.IsReadOnly))
                    {
                        foreach (var pair in dictionary.Keys.Cast<object>().Select(key => (key, value: dictionary[key])).ToArray())
                        {
                            var keySegment = $"{pair.key.GetHashCode()}.key";
                            var valueSegment = $"{pair.key.GetHashCode()}.value";

                            using (Horizontal())
                            {
                                using (Vertical())
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var modifiedKey = Object("Key", pair.key, element.key, path.Append(keySegment).ToArray());
                                    if (EditorGUI.EndChangeCheck())
                                    {
                                        dictionary.Remove(pair.key);
                                        dictionary[modifiedKey] = pair.value;
                                    }
                                }

                                using (Vertical())
                                {
                                    EditorGUI.BeginChangeCheck();
                                    var modifiedValue = Object("Value", pair.value, element.value, path.Append(valueSegment).ToArray());
                                    if (EditorGUI.EndChangeCheck()) dictionary[pair.key] = modifiedValue;
                                }
                            }
                        }
                    }
                }
            }

            return dictionary;
        }
    }
}
