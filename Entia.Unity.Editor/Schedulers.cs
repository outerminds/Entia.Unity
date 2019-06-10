using System;
using Entia.Modules.Schedule;
using Entia.Systems;
using UnityEditor;

namespace Entia.Schedulers
{
    public sealed class DrawGizmo : Scheduler<IDrawGizmo>
    {
        public override Type[] Phases => new[] { typeof(Phases.DrawGizmo) };
        public override Phase[] Schedule(IDrawGizmo instance, Controller controller)
        {
            var draw = new Action<GizmoType>(instance.DrawGizmo);
            return new[] { Phase.From((in Phases.DrawGizmo phase) => draw(phase.Type)) };
        }
    }
}