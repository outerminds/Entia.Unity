using System;
using System.Collections.Generic;
using System.Linq;
using Entia.Core;
using Entia.Modules;
using Entia.Nodes;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entia.Unity
{
    public interface IWorldReference
    {
        World World { get; }
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
        Scene _scene;
        bool _initialized;
        bool _disposed;

        void Awake() => Initialize();
        void OnDestroy() => Dispose();

        public World Create()
        {
            var world = new World();
            world.Builders().Set<Profile>(new Builders.Profile());
            world.Builders().Set<Parallel>(new Builders.Parallel());
            world.Analyzers().Set(new Analyzers.Parallel());
            world.Cloners().Set(new Cloners.Object());
            foreach (var modifier in _modifiers) modifier?.Modify(world);
            return world;
        }

        public void Initialize()
        {
            if (_initialized.Change(true))
            {
                World = Create();
                _scene = gameObject.scene;
                SceneManager.sceneUnloaded += Unload;
                Application.quitting += Dispose;

                var roots = _scene.GetRootGameObjects();
                foreach (var resource in roots.SelectMany(root => root.GetComponentsInChildren<IResourceReference>()))
                    resource.Initialize(World);

                foreach (var controller in roots.SelectMany(root => root.GetComponentsInChildren<IControllerReference>()))
                    controller.Initialize(World);

                var entities = roots.SelectMany(root => root.GetComponentsInChildren<IEntityReference>()).ToArray();
                foreach (var entity in entities) entity.PreInitialize();
                foreach (var entity in entities) entity.Initialize(World, false);
                foreach (var entity in entities) entity.PostInitialize();

                WorldRegistry.Set(_scene, this);
            }
        }

        public void Dispose()
        {
            if (_initialized && _disposed.Change(true))
            {
                var roots = _scene.GetRootGameObjects();

                var entities = roots.SelectMany(root => root.GetComponentsInChildren<IEntityReference>()).ToArray();
                foreach (var entity in entities) entity.PreDispose();
                foreach (var entity in entities) entity.Dispose();
                foreach (var entity in entities) entity.PostDispose();

                foreach (var controller in roots.SelectMany(root => root.GetComponentsInChildren<IControllerReference>()))
                    controller.Dispose();

                foreach (var resource in roots.SelectMany(root => root.GetComponentsInChildren<IResourceReference>()))
                    resource.Dispose();

                WorldRegistry.Remove(_scene);
                SceneManager.sceneUnloaded -= Unload;
                Application.quitting -= Dispose;
                World = null;
                _initialized = false;
                _disposed = false;
            }
        }

        void Unload(Scene scene) { if (scene == _scene) Dispose(); }
    }
}