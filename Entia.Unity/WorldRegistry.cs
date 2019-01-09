using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Entia.Unity
{
    public static class WorldRegistry
    {
        sealed class Comparer : IEqualityComparer<Scene>
        {
            public bool Equals(Scene x, Scene y) => x == y;
            public int GetHashCode(Scene obj) => obj.GetHashCode();
        }

        public static IEnumerable<IWorldReference> References => _worlds.Values;

        static readonly Dictionary<Scene, IWorldReference> _worlds = new Dictionary<Scene, IWorldReference>(new Comparer());

        public static bool TryGet(Scene scene, out IWorldReference world) => _worlds.TryGetValue(scene, out world);

        public static bool Has(Scene scene) => _worlds.ContainsKey(scene);
        public static bool Has(IWorldReference reference) => _worlds.ContainsValue(reference);

        public static bool Set(Scene scene, IWorldReference reference)
        {
            if (_worlds.ContainsKey(scene)) return false;
            _worlds.Add(scene, reference);
            return true;
        }

        public static bool Remove(Scene scene) => _worlds.Remove(scene);
    }
}