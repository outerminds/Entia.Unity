using Entia.Modules;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(EntityReference), isFallback = true)]
    public class EntityReferenceEditor : UnityEditor.Editor
    {
        public IEntityReference Target => target as IEntityReference;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (Target.World is World world)
                world.ShowEntity(Target.Entity.ToString(world), Target.Entity, nameof(EntityReferenceEditor), Target.Entity.ToString());
        }
    }
}
