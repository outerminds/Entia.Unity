using Entia.Injectables;
using Entia.Messages;
using Entia.Systems;
using Entia.Components;
using System.Collections.Generic;
using UnityEngine;
using Entia.Core;

namespace Entia.Unity.Systems
{
    public readonly struct SynchronizeGameObject : IReact<OnRemove<Unity<GameObject>>>
    {
        public readonly Components<Unity<GameObject>> GameObjects;

        void IReact<OnRemove<Unity<GameObject>>>.React(in OnRemove<Unity<GameObject>> message)
        {
            ref var @object = ref GameObjects.GetOrDummy(message.Entity, out var success);
            if (success && @object.Value != null)
            {
                Object.Destroy(@object.Value);
                @object.Value = null;
            }
        }
    }
}
