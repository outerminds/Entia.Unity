using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class AboutMenu
    {
        [MenuItem("Entia/About", priority = 5000)]
        static void About() => Debug.Log($"Entia: {typeof(Entity).Assembly.GetName().Version} | Entia.Unity: {typeof(EntityReference).Assembly.GetName().Version}");
    }
}