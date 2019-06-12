using Entia.Modules;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(EntityReference), isFallback = true)]
    public class EntityReferenceEditor : UnityEditor.Editor
    {
        public IEntityReference Target => target as IEntityReference;

        protected virtual void OnEnable() => ReferenceUtility.Update();

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            using (ReferenceUtility.Entity(Target)) { }
        }
    }
}
