using Entia.Unity.Queryables;

namespace Entia.Unity
{
    public static class ItemExtensions
    {
        public static void Deconstruct<T>(in this Unity<T> item, out T value) where T : UnityEngine.Object => value = item.Value;
    }
}