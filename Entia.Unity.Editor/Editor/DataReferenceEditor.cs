using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entia.Unity.Editor
{
    public abstract class DataReferenceEditor<T> : UnityEditor.Editor where T : class
    {
        public T Target => target as T;
        public IEnumerable<T> Targets => targets.OfType<T>();

        public override bool RequiresConstantRepaint() => true;

        protected virtual void OnInspectorGUI(SerializedProperty property)
        {
            while (property.NextVisible(false)) EditorGUILayout.PropertyField(property, true);
        }
    }
}
