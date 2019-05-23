using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entia.Unity.Editor
{
    public abstract class ComponentReferenceEditor<T> : UnityEditor.Editor where T : class, IComponentReference
    {
        public T Target => target as T;
        public IEnumerable<T> Targets => targets.OfType<T>();

        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.Component(serializedObject, Targets, out var property))
                OnComponentInspectorGUI(property);
        }

        protected virtual void OnComponentInspectorGUI(SerializedProperty property)
        {
            while (property.NextVisible(false)) EditorGUILayout.PropertyField(property, true);
        }
    }

    [CustomEditor(typeof(ComponentReference), true, isFallback = true), CanEditMultipleObjects]
    public class ComponentReferenceEditor : ComponentReferenceEditor<IComponentReference> { }
}
