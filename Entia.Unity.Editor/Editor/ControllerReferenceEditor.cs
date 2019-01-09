using Entia.Nodes;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(ControllerReference), true, isFallback = true)]
    public class ControllerReferenceEditor : UnityEditor.Editor
    {
        static bool _details;

        public World World => Target?.World ?? Target.GetComponent<IWorldReference>()?.Create();
        public Node Node => Target?.Node;
        public ControllerReference Target => target as ControllerReference;

        public override void OnInspectorGUI()
        {
            using (LayoutUtility.Disable(EditorApplication.isPlayingOrWillChangePlaymode)) base.OnInspectorGUI();

            if (World is World world)
            {
                using (LayoutUtility.Horizontal())
                {
                    EditorGUILayout.LabelField(nameof(Node), LayoutUtility.BoldLabel);
                    _details = LayoutUtility.Toggle("Details", _details);
                }
                using (LayoutUtility.Indent()) world?.ShowNode(Node, _details);
            }
        }
    }
}
