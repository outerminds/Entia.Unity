using Entia.Messages;
using Entia.Modules;
using UnityEditor;

namespace Entia.Unity.Editor
{
    public static class DrawGizmos
    {
        [DrawGizmo((GizmoType)~0)]
        public static void WorldReference(WorldReference world, GizmoType type) =>
            world.World?.Messages().Emit(new OnDrawGizmo { Type = type });
    }
}