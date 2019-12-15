using Components;
using Entia;
using Entia.Components;
using Entia.Messages;
using Entia.Systems;
using UnityEngine;

namespace Systems
{
    // 'OnDrawGizmo' messages are emitted at draw gizmo time, thus the 'Gizmos' api can be used
    public struct DrawVelocity : IReactEach<OnDrawGizmo, Unity<Transform>, Components.Velocity>
    {
        public void React(in OnDrawGizmo message, Entity entity, ref Unity<Transform> transform, ref Components.Velocity velocity) =>
            Gizmos.DrawRay(transform.Value.position, new Vector3(velocity.X, velocity.Y) * 3f);
    }

    public struct SynchronizePosition : IRunEach<Unity<Transform>, Position>, IReactEach<OnValidate<Components.Generated.Position>>
    {
        public void Run(Entity entity, ref Unity<Transform> transform, ref Position position) =>
            position = transform.Value.position;

        public void React(in OnValidate<Components.Generated.Position> message, Entity entity) =>
            message.Reference.transform.position = message.Reference.Value;
    }
}