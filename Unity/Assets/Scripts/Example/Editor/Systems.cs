using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entia.Unity.Systems;
using Entia.Systems;
using Entia.Injectables;
using Entia.Queryables;
using UnityEditor;

namespace Systems
{
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
