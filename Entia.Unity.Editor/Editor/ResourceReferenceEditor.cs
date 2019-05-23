using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entia.Unity.Editor
{
    public abstract class ResourceReferenceEditor<T> : UnityEditor.Editor where T : class, IResourceReference
    {
        public IResourceReference Target => target as IResourceReference;
        public IEnumerable<IResourceReference> Targets => targets.OfType<IResourceReference>();

        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.Resource(serializedObject, Targets, out var property))
                OnResourceInspectorGUI(property);
        }

        protected virtual void OnResourceInspectorGUI(SerializedProperty property)
        {
            while (property.NextVisible(false)) EditorGUILayout.PropertyField(property, true);
        }
    }

    [CustomEditor(typeof(ResourceReference), true, isFallback = true), CanEditMultipleObjects]
    public class ResourceReferenceEditor : ResourceReferenceEditor<IResourceReference> { }
}
