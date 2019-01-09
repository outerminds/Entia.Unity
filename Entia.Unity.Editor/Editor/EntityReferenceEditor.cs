using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(EntityReference), isFallback = true)]
    public class EntityReferenceEditor : UnityEditor.Editor
    {
        public IEntityReference Target => target as IEntityReference;
        public World World => Target?.World;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            World?.ShowEntity(World.Name(Target.Entity), Target.Entity, nameof(EntityReferenceEditor), Target.Entity.ToString());
        }
    }
}
