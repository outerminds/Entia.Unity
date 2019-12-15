using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entia.Unity.Editor
{
    public abstract class ResourceReferenceEditor<T> : DataReferenceEditor<T> where T : class, IResourceReference
    {
        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.Resource(serializedObject, Targets, out var property))
                OnInspectorGUI(property);
        }
    }

    [CustomEditor(typeof(ResourceReference), true, isFallback = true), CanEditMultipleObjects]
    public class ResourceReferenceEditor : ResourceReferenceEditor<IResourceReference> { }
}
