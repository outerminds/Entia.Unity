using System.Collections.Generic;
using UnityEngine;

namespace Entia.Unity
{
    public static class WorldExtensions
    {
        public static bool TryWorld(this GameObject gameObject, out World world)
        {
            if (WorldRegistry.TryGet(gameObject.scene, out var reference))
            {
                world = reference.World;
                return true;
            }

            world = default;
            return false;
        }

        public static bool TryWorld(this Component component, out World world) => component.gameObject.TryWorld(out world);
    }
}
