using Entia.Injectables;
using Entia.Messages;
using Entia.Queryables;
using Entia.Systems;
using Entia.Unity;
using UnityEngine;

namespace Systems
{
    // 'OnDrawGizmo' messages are emitted at draw gizmo time, thus the 'Gizmos' api can be used
    public struct DrawVelocity : IReact<OnDrawGizmo>
    {
        public Group<Unity<Transform>, Read<Components.Velocity>> Group;

        public void React(in OnDrawGizmo message)
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