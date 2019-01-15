using Entia.Core;
using Entia.Modules;
using Entia.Modules.Control;
using Entia.Nodes;
using Entia.Phases;
using Entia.Unity.Phases;
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

        void Initialize(World world);
        void Dispose();
    }

    [RequireComponent(typeof(WorldReference))]
    public abstract class ControllerReference : MonoBehaviour, IControllerReference
    {
        public World World => Controller?.World;
        public Controller Controller { get; private set; }
        public abstract Node Node { get; }

        bool _initialized;
        bool _disposed;

        protected virtual Result<Controller> Create(World world)
        {
            var node = Application.isEditor || Debug.isDebugBuild ? Node.Profile() : Node;
            return world.Controllers().Control(node.Separate(Entia.Nodes.Node.Resolve));
        }

        void Awake()
        {
            if (WorldRegistry.TryGet(gameObject.scene, out var reference) && reference.World is World world)
                Initialize(world);
        }

        void OnDestroy() => Dispose();

        protected virtual void OnEnable() => Controller?.Enable();
        protected virtual void OnDisable() => Controller?.Disable();
        protected virtual void Update() => Controller?.Run();
        protected virtual void FixedUpdate() => Controller?.Run<RunFixed>();
        protected virtual void LateUpdate() => Controller?.Run<RunLate>();

        void Initialize(World world)
        {
            if (_initialized.Change(true))
            {
                var result = Create(world);
                if (result.TryMessages(out var messages))
                {
                    var log = string.Join(
                        Environment.NewLine,
                        messages
                            .Distinct()
                            .Select(message => $"-> {message}")
                            .Prepend($"Failed to create controller '{GetType().Format()}'. See details below."));
                    Debug.LogError(log);
                }

                Controller = result.Or(default(Controller));
                Controller?.Initialize();
            }
        }

        void Dispose()
        {
            if (_initialized && _disposed.Change(true)) Controller?.Dispose();
        }

        void IControllerReference.Initialize(World world) => Initialize(world);
        void IControllerReference.Dispose() => Dispose();
    }
}