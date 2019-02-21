using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(ComponentReference), true, isFallback = true), CanEditMultipleObjects]
    public class ComponentReferenceEditor : UnityEditor.Editor
    {
        public IComponentReference Target => target as IComponentReference;
        public IEnumerable<IComponentReference> Targets => targets.OfType<IComponentReference>();

        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.Component(serializedObject, Targets, out var property))
                while (property.NextVisible(false)) EditorGUILayout.PropertyField(property, true);
        }
    }
}
