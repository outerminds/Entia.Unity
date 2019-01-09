using System;
using UnityEditor;

namespace Entia.Unity
{
    public static class EditorUtility
    {
        public static void Delayed(Action action, int delay)
        {
            if (delay <= 0) action();
            else
            {
                var callback = default(EditorApplication.CallbackFunction);
                callback = new EditorApplication.CallbackFunction(() =>
                {
                    if (delay-- > 0) return;
                    action();
                    EditorApplication.update -= callback;
                });

                EditorApplication.update += callback;
            }
        }
    }
}