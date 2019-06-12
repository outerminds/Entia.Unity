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
        INodeReference[] Nodes { get; }

        void Initialize(World world);
        void Dispose();
    }

    [RequireComponent(typeof(WorldReference))]
    public sealed class ControllerReference : MonoBehaviour, IControllerReference
    {
        public World World => Controller?.World;
        public Controller Controller { get; private set; }
        public INodeReference[] Nodes => _nodes;

        [SerializeField]
        NodeReference[] _nodes = { };
        [NonSerialized]
        bool _initialized;
        [NonSerialized]
        bool _disposed;
        [NonSerialized]
        string _log = "-> No details available.";

        public Node Create()
        {
            var children = _nodes
                .Select(reference =>
                {
                    if (reference == null) return default;
                    var child = reference.Node;
                    return
                        string.IsNullOrWhiteSpace(child.Name) ?
                            string.IsNullOrWhiteSpace(reference.name) ?
                                child.With(reference.GetType().Format()) : child.With(reference.name) :
                        child;
                })
                .Some()
                .ToArray();
            var node = Node.Sequence(name, children);
            if (Debug.isDebugBuild) node = node.Profile();
            return node;
        }

        void Awake()
        {
            if (gameObject.TryWorld(out var world)) Initialize(world);
        }

        void OnDestroy() => Dispose();

        void OnEnable() => Enable();
        void OnDisable() => Disable();
        void Update() => Run<Run>();
        void FixedUpdate() => Run<RunFixed>();
        void LateUpdate() => Run<RunLate>();

        void Initialize(World world)
        {
            if (world == null) return;
            if (!Application.isPlaying || _initialized.Change(true))
            {
                var node = Create();
                // NOTE: the 'Try' ensures that early crashes are still caught and logged
                var result = Result.Try(node, world.Controllers().Control).Flatten();
                if (result.TryMessages(out var messages))
                {
                    _log = string.Join(Environment.NewLine, messages.Distinct().Select(message => $"-> {message}"));
                    Debug.LogError(
$@"Failed to create controller for node '{node}'. See details below.
{_log}");
                }

                Controller = result.OrDefault();

                if (Application.isPlaying)
                {
                    Run<Initialize>();
                    Run<React.Initialize>();
                }
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