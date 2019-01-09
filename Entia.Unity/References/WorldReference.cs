using System;
using System.Linq;
using Entia.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entia.Unity
{
    public interface IWorldReference
    {
        World World { get; }
        World Create();
    }

    [DisallowMultipleComponent]
    public sealed class WorldReference : MonoBehaviour, IWorldReference
    {
        static readonly Lazy<WorldFactory> _default = new Lazy<WorldFactory>(ScriptableObject.CreateInstance<WorldFactory>);

        public World World { get; private set; }
        public WorldFactory Factory;

        Scene _scene;
        bool _initialized;
        bool _disposed;

        void Awake() => Initialize();
        void OnDestroy() => Dispose();

        public World Create() => Factory?.Create() ?? _default.Value.Create();

        void Initialize()
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
                foreach (var entity in entities) entity.Initialize(World);
                foreach (var entity in entities) entity.PostInitialize();

                WorldRegistry.Set(_scene, this);
            }
        }

        void Unload(Scene scene) { if (scene == _scene) Dispose(); }

        void Dispose()
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
            }
        }
    }
}