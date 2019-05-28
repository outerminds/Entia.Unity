using Entia.Injectables;
using Entia.Messages;
using Entia.Systems;
using Entia.Components;
using System.Collections.Generic;
using UnityEngine;
using Entia.Core;

namespace Entia.Unity.Systems
{
    public readonly struct SynchronizeEntityReference : IReact<OnRemove<Unity<EntityReference>>>
    {
        public readonly Components<Unity<EntityReference>> References;

        void IReact<OnRemove<Unity<EntityReference>>>.React(in OnRemove<Unity<EntityReference>> message)
        {
            if (References.TryUnity(message.Entity, out var reference) && reference)
                Object.Destroy(reference.gameObject);
        }
    }
}
