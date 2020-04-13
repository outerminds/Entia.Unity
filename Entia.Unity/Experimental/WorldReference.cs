using System;
using Entia.Core;
using Entia.Experimental.Phases;
using Entia.Experimental.Scheduling;
using Entia.Modules;
using Entia.Modules.Message;
using UnityEngine;

namespace Entia.Experimental.Unity
{
    public sealed class WorldReference : MonoBehaviour
    {
        sealed class State
        {
            public World World;
            public Node Node;
            public Emitter<OnInitialize> OnInitialize;
            public Emitter<OnDispose> OnDispose;
            public Emitter<OnUpdate> OnUpdate;
            public Emitter<OnFixedUpdate> OnFixedUpdate;
            public Emitter<OnLateUpdate> OnLateUpdate;
        }

        public World World => _state.Value.World;
        public Node Node => _state.Value.Node;

        [SerializeField] WorldModifier[] _modifiers = { };
        [SerializeField] NodeReference[] _nodes = { };
        [NonSerialized] bool _initialized, _disposed;
        Lazy<State> _state;
        Disposable _disposable = Disposable.Empty;

        public WorldReference() => _state = new Lazy<State>(CreateState);

        void Awake() { Initialize(); }
        void OnDestroy() { Dispose(); }
        void Update() { _state.Value.OnUpdate.Emit(); }
        void FixedUpdate() { _state.Value.OnFixedUpdate.Emit(); }
        void LateUpdate() { _state.Value.OnLateUpdate.Emit(); }

        public void Initialize()
        {
            if (_initialized.Change(true))
            {
                _disposed = false;
                var state = _state.Value;
                var result = state.World.Schedule(state.Node);
                if (result.TryMessages(out var messages))
                {
                    var errors = messages.Select(message => string.Format($"{Environment.NewLine}-> {message}"));
                    Debug.LogError($"Failed to schedule node:{string.Join("", errors)}");
                }
                _disposable = result.Or(Disposable.Empty);
                state.OnInitialize.Emit();
            }
        }

        public void Dispose()
        {
            if (_disposed.Change(true) && _initialized.Change(false))
            {
                _state.Value.OnDispose.Emit();
                _disposable.Dispose();
                _disposable = Disposable.Empty;
                _state = new Lazy<State>(CreateState);
            }
        }

        State CreateState()
        {
            var world = new World();
            var node = Node.Sequence(_nodes.Select(node => node.Node));
            var resources = world.Resources();
            var messages = world.Messages();
            if (Debug.isDebugBuild) resources.Set(new Resources.Debug { Name = $"{{ Name: {name}, Scene: {gameObject.scene.name} }}" });
            // resources.Set(new Resources.Unity { Scene = gameObject.scene, Reference = this });
            foreach (var modifier in _modifiers) modifier.Modify(world);
            return new State
            {
                World = world,
                Node = node,
                OnInitialize = messages.Emitter<OnInitialize>(),
                OnDispose = messages.Emitter<OnDispose>(),
                OnUpdate = messages.Emitter<OnUpdate>(),
                OnFixedUpdate = messages.Emitter<OnFixedUpdate>(),
                OnLateUpdate = messages.Emitter<OnLateUpdate>(),
            };
        }
    }
}