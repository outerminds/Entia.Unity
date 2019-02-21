using System;
using UnityEditor;

namespace Entia.Unity
{
    public static class EditorUtility
    {
        public static void Delayed(Action action, Func<int, bool> condition)
        {
            if (condition(0)) action();
            else
            {
                var count = 0;
                var callback = default(EditorApplication.CallbackFunction);
                EditorApplication.update += callback = () =>
                {
                    if (condition(++count))
                    {
                        action();
                        EditorApplication.update -= callback;
                    }
                };
            }
        }

        public static void Delayed(Action action, int delay = 1) => Delayed(action, count => count >= delay);
    }
}