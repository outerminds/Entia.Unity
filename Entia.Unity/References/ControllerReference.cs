using Entia.Core;
using Entia.Modules;
using Entia.Nodes;
using Entia.Phases;
using System;
using System.Linq;
using UnityEngine;

namespace Entia.Unity
{
    public interface IControllerReference
    {
        World World { get; }
        Controller Controller { get; }
        Node Node { get; }
        INodeModifier[] Modifiers { get; }

        void Initialize(World world);
        void Dispose();
    }

    [RequireComponent(typeof(WorldReference))]
    public abstract class ControllerReference : MonoBehaviour, IControllerReference
    {
        public World World => Controller?.World;
        public Controller Controller { get; private set; }
        public abstract Node Node { get; }
        public INodeModifier[] Modifiers => _modifiers;

        [SerializeField]
        NodeModifier[] _modifiers = { };
        bool _initialized;
        bool _disposed;
        string _log = "-> No details available.";

        protected virtual Result<Controller> Create(World world)
        {
            var node = Node;
            foreach (var modifier in _modifiers) node = modifier?.Modify(node) ?? node;
            return world.Controllers().Control(node);
        }

        void Awake()
        {
            if (gameObject.TryWorld(out var world)) Initialize(world);
        }

        void OnDestroy() => Dispose();

        protected virtual void OnEnable() => Enable();
        protected virtual void OnDisable() => Disable();
        protected virtual void Update() => Run<Run>();
        protected virtual void FixedUpdate() => Run<RunFixed>();
        protected virtual void LateUpdate() => Run<RunLate>();
        protected virtual void Reset() => _modifiers = new[] { ScriptableObject.CreateInstance<ProfileModifier>() };

        void Initialize(World world)
        {
            if (_initialized.Change(true))
            {
                var result = Result.Try(() => Create(world)).Flatten();
                if (result.TryMessages(out var messages))
                {
                    _log = string.Join(Environment.NewLine, messages.Distinct().Select(message => $"-> {message}"));
                    Debug.LogError(
$@"Failed to create controller '{GetType().Format()}' for node {Node}. See details below.
{_log}");
                }

                Controller = result.OrDefault();
                Run<Initialize>();
                Run<React.Initialize>();
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                Run<React.Dispose>();
                Run<Dispose>();
                _initialized = false;
                _disposed = false;
            }
        }

        void Enable()
        {
            if (Controller == null)
            {
                if (_initialized) Debug.LogError(
$@"Failed to enable 'null' controller. See details below.
{_log}");
            }
            else Controller.Enable();
        }

        void Disable()
        {
            if (Controller == null)
            {
                if (_initialized) Debug.LogError(
$@"Failed to disable 'null' controller. See details below.
{_log}");
            }
            else Controller.Disable();
        }

        void Run<T>() where T : struct, IPhase
        {
            if (Controller == null)
            {
                Debug.LogError(
$@"Failed to run phase of type '{typeof(T).Format()}' since the controller is 'null'. See details below.
{_log}");
            }
            else Controller.Run<T>();
        }

        void IControllerReference.Initialize(World world) => Initialize(world);
        void IControllerReference.Dispose() => Dispose();
    }
}