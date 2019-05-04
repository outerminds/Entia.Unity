using System.Collections.Generic;
using Entia.Modules;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Entia.Unity
{
    public static class WorldExtensions
    {
        public static bool TryWorld(this GameObject gameObject, out World world) => gameObject.scene.TryWorld(out world);
        public static bool TryWorld(this Component component, out World world) => component.gameObject.TryWorld(out world);
        public static bool TryWorld(this Scene scene, out World world) =>
            World.TryInstance(scene, (instance, state) => instance.TryScene(out var other) && state == other, out world);

        public static bool TryScene(this World world, out Scene scene) => world.Resources().TryScene(out scene);
        public static bool TryScene(this Modules.Resources resources, out Scene scene)
        {
            if (resources.TryGet<Resources.Unity>(out var unity))
            {
                scene = unity.Scene;
                return true;
            }

            scene = default;
            return false;
        }
    }
}
