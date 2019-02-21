using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(ResourceReference), true, isFallback = true), CanEditMultipleObjects]
    public class ResourceReferenceEditor : UnityEditor.Editor
    {
        public IResourceReference Target => target as IResourceReference;
        public IEnumerable<IResourceReference> Targets => targets.OfType<IResourceReference>();

        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.Resource(serializedObject, Targets, out var property))
                while (property.NextVisible(false)) EditorGUILayout.PropertyField(property, true);
        }
    }
}
