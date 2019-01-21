using Entia.Injectables;
using Entia.Messages;
using Entia.Systems;
using Entia.Components;
using System.Collections.Generic;
using UnityEngine;

namespace Entia.Unity.Systems
{
    public struct SynchronizeGameObject : IInitialize, IReact<OnAdd<Unity<GameObject>>>, IReact<OnRemove<Unity<GameObject>>>
    {
        public Components<Unity<GameObject>> GameObjects;

        Dictionary<Entity, GameObject> _gameObjects;

        void IInitialize.Initialize() => _gameObjects = new Dictionary<Entity, GameObject>();

        void IReact<OnAdd<Unity<GameObject>>>.React(in OnAdd<Unity<GameObject>> message)
        {
            ref var gameObject = ref GameObjects.GetOrDummy(message.Entity, out var success);
            if (success) _gameObjects[message.Entity] = gameObject.Value;
        }

        void IReact<OnRemove<Unity<GameObject>>>.React(in OnRemove<Unity<GameObject>> message)
        {
            if (_gameObjects.TryGetValue(message.Entity, out var gameObject))
            {
                _gameObjects.Remove(message.Entity);
                if (gameObject != null) Object.Destroy(gameObject);
            }
        }
    }
}
