using System;
using Entia.Core;
using Entia.Experimental.Phases;
using Entia.Modules;
using Entia.Modules.Message;
using Entia.Unity;
using UnityEngine;

namespace Entia.Experimental.Unity
{
    [RequireComponent(typeof(WorldReference))]
    public class ControllerReference : MonoBehaviour
    {
        public NodeReference[] Nodes => _nodes;
        public Option<World> World => _world;

        [SerializeField] NodeReference[] _nodes = { };
        [NonSerialized] bool _initialized;
        [NonSerialized] bool _disposed;
        [NonSerialized] Option<World> _world;

        Emitter<OnEnable> _onEnable;
        Emitter<OnDisable> _onDisable;
        Emitter<OnAwake> _onAwake;
        Emitter<OnDestroy> _onDestroy;
        Emitter<OnStart> _onStart;
        Emitter<OnUpdate> _onUpdate;
        Emitter<OnFixedUpdate> _onFixedUpdate;
        Emitter<OnLateUpdate> _onLateUpdate;

        protected virtual void OnEnable() { Initialize(); _onEnable?.Emit(); }
        protected virtual void OnDisable() { _onDisable?.Emit(); }
        protected virtual void Awake() { Initialize(); _onAwake?.Emit(); }
        protected virtual void OnDestroy() { _onDestroy?.Emit(); Dispose(); }
        protected virtual void Start() { _onStart?.Emit(); }
        protected virtual void Update() { _onUpdate?.Emit(); }
        protected virtual void FixedUpdate() { _onFixedUpdate?.Emit(); }
        protected virtual void LateUpdate() { _onLateUpdate?.Emit(); }

        public void Initialize()
        {
            if (_initialized.Change(true) && GetComponent<WorldReference>() is WorldReference reference && reference.World is World world)
            {
                var messages = world.Messages();
                _onEnable = messages.Emitter<OnEnable>();
                _onDisable = messages.Emitter<OnDisable>();
                _onAwake = messages.Emitter<OnAwake>();
                _onDestroy = messages.Emitter<OnDestroy>();
                _onStart = messages.Emitter<OnStart>();
                _onUpdate = messages.Emitter<OnUpdate>();
                _onFixedUpdate = messages.Emitter<OnFixedUpdate>();
                _onLateUpdate = messages.Emitter<OnLateUpdate>();
                _world = world;
                _disposed = false;
                messages.Emit<OnInitialize>();
            }
        }

        public void Dispose()
        {
            if (_disposed.Change(true) && _initialized.Change(false) && _world.TryValue(out var world))
            {
                var messages = world.Messages();
                _world = null;
                messages.Emit<OnDispose>();
            }
        }
    }
}