using System;
using System.Linq;
using Entia.Core;
using Entia.Modules;
using Entia.Nodes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entia.Unity
{
    public interface IWorldReference : IReference
    {
        IWorldModifier[] Modifiers { get; }
        World Create();
        void Initialize();
        void Dispose();
    }

    [DisallowMultipleComponent]
    public sealed class WorldReference : MonoBehaviour, IWorldReference
    {
        public World World { get; private set; }
        public IWorldModifier[] Modifiers => _modifiers;

        [SerializeField]
        WorldModifier[] _modifiers = { };
        [NonSerialized]
        bool _initialized;
        [NonSerialized]
        bool _disposed;

        void Awake() => Initialize();
        void OnDestroy() => Dispose();

        public World Create()
        {
            var world = new World();
            world.Container.Add<Profile, Builders.Profile>(new Builders.Profile());
            world.Container.Add<Parallel, Builders.Parallel>(new Builders.Parallel());
            world.Container.Add<Parallel, Analyzers.Parallel>(new Analyzers.Parallel());

            var resources = world.Resources();
            if (UnityEngine.Debug.isDebugBuild) resources.Set(new Resources.Debug { Name = $"{{ Name: {name}, Scene: {gameObject.scene.name} }}" });
            resources.Set(new Resources.Unity { Scene = gameObject.scene, Reference = this });

            foreach (var modifier in _modifiers) modifier?.Modify(world);
            return world;
        }

        public void Initialize()
        {
            if (!Application.isPlaying || _initialized.Change(true))
            {
                World = Create();
                World.TryScene(out var scene);
                SceneManager.sceneUnloaded += Unload;
                Application.quitting += Dispose;

                var roots = scene.GetRootGameObjects();
                foreach (var resource in roots.SelectMany(root => root.GetComponentsInChildren<IResourceReference>()))
                    resource.Initialize(World);

                foreach (var controller in roots.SelectMany(root => root.GetComponentsInChildren<IControllerReference>()))
                    controller.Initialize(World);

                var entities = roots.SelectMany(root => root.GetComponentsInChildren<IEntityReference>()).ToArray();
                foreach (var entity in entities) entity.PreInitialize();
                foreach (var entity in entities) entity.Initialize(World);
                foreach (var entity in entities) entity.PostInitialize();
                World.Resolve();
            }
        }

        public void Dispose()
        {
            if (World == null) return;

            var resources = World.Resources();
            if (_initialized && resources.TryScene(out var scene) && _disposed.Change(true))
            {
                var roots = scene.GetRootGameObjects();
                var entities = roots.SelectMany(root => root.GetComponentsInChildren<IEntityReference>()).ToArray();
                foreach (var entity in entities) entity.PreDispose();
                foreach (var entity in entities) entity.Dispose();
                foreach (var entity in entities) entity.PostDispose();

                foreach (var controller in roots.SelectMany(root => root.GetComponentsInChildren<IControllerReference>()))
                    controller.Dispose();

                foreach (var resource in roots.SelectMany(root => root.GetComponentsInChildren<IResourceReference>()))
                    resource.Dispose();

                // NOTE: remove the resource in case the world is accessed through the static registry before it is GC'd
                resources.Remove<Resources.Unity>();
                SceneManager.sceneUnloaded -= Unload;
                Application.quitting -= Dispose;
                World = null;
                _initialized = false;
                _disposed = false;
            }
        }

        void Unload(Scene scene)
        {
            if (World == null) return;
            if (World.TryScene(out var other) && scene == other) Dispose();
        }
    }
}