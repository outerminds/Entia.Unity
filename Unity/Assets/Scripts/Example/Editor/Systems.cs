using Entia.Injectables;
using Entia.Queryables;
using Entia.Systems;
using UnityEditor;
using UnityEngine;

namespace Systems
{
    // 'IDrawGizmo' systems are called at draw gizmo time, thus the 'Gizmos' api can be used
    public struct DrawVelocity : IDrawGizmo
    {
        public Group<Unity<Transform>, Read<Components.Velocity>> Group;

        void IDrawGizmo.DrawGizmo(GizmoType type)
        {
            foreach (ref readonly var item in Group)
            {
                var transform = item.Transform();
                ref readonly var velocity = ref item.Velocity();
                Gizmos.DrawRay(transform.position, new Vector3(velocity.X, velocity.Y) * 3f);
            }
        }
    }
}