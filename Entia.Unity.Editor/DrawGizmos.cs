using UnityEditor;

namespace Entia.Unity.Editor
{
    static class DrawGizmos
    {
        [DrawGizmo((GizmoType)~0)]
        public static void ControllerReference(ControllerReference controller, GizmoType type) =>
            controller.Controller?.Run<Phases.DrawGizmo>(new Phases.DrawGizmo { Type = type });
    }
}