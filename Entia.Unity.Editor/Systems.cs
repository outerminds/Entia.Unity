using Entia.Schedulables;
using UnityEditor;

namespace Entia.Systems
{
    public interface IDrawGizmo : ISystem, ISchedulable<Schedulers.DrawGizmo>
    {
        void DrawGizmo(GizmoType type);
    }
}