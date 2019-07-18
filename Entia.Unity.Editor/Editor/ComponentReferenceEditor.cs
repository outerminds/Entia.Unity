using UnityEditor;

namespace Entia.Unity.Editor
{
    public abstract class ComponentReferenceEditor<T> : DataReferenceEditor<T> where T : class, IComponentReference
    {
        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.Component(serializedObject, Targets, out var property))
                OnInspectorGUI(property);
        }
    }

    [CustomEditor(typeof(ComponentReference), true, isFallback = true), CanEditMultipleObjects]
    public class ComponentReferenceEditor : ComponentReferenceEditor<IComponentReference> { }
}
