using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(TagReference), true, isFallback = true), CanEditMultipleObjects]
    public class TagReferenceEditor : UnityEditor.Editor
    {
        public ITagReference Target => target as ITagReference;
        public IEnumerable<ITagReference> Targets => targets.OfType<ITagReference>();

        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.Tag(Targets))
            using (LayoutUtility.Apply(serializedObject))
            {
                var property = ReferenceUtility.Script(serializedObject);
                while (property.NextVisible(false)) EditorGUILayout.PropertyField(property, true);
            }
        }
    }
}
