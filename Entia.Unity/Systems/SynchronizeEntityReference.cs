using Entia.Injectables;
using Entia.Messages;
using Entia.Systems;
using Entia.Components;
using UnityEngine;

namespace Entia.Unity.Systems
{
    public readonly struct SynchronizeEntityReference : IOnRemove<Unity<EntityReference>>
    {
        void IOnRemove<Unity<EntityReference>>.OnRemove(Entity entity, ref Unity<EntityReference> component) =>
            Object.Destroy(component.Value.gameObject);
    }
}
