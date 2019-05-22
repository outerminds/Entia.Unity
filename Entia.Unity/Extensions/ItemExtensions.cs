using Entia.Queryables;

namespace Entia.Unity
{
    public static class ItemExtensions
    {
        public static bool TryGet<T>(in this Maybe<Unity<T>> item, out T value) where T : UnityEngine.Object
        {
            value = item.Value.Value;
            return value != null;
        }
        public static T Get<T>(in this Maybe<Unity<T>> item, out bool success) where T : UnityEngine.Object
        {
            var value = item.Value.Value;
            success = value != null;
            return value;
        }
        public static void Deconstruct<T>(in this Unity<T> item, out T value) where T : UnityEngine.Object => value = item.Value;
    }
}