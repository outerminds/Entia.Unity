using System.Collections.Generic;
using Entia.Core;

namespace Entia.Unity
{
    public static class PoolUtility
    {
        public static class Cache<T>
        {
            public static readonly Pool<List<T>> Lists = new Pool<List<T>>(() => new List<T>(), dispose: instance => instance.Clear());
        }
    }
}