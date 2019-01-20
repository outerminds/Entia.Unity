using Entia.Nodes;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(ControllerReference), true, isFallback = true)]
    public class ControllerReferenceEditor : UnityEditor.Editor
    {
        static bool _details;

        public IWorldReference Reference => _reference ?? (_reference = Target.GetComponent<IWorldReference>());
        public World World => Target?.World ?? Reference?.World ?? _world ?? (_world = Reference?.Create());
        public Node Node => Target?.Node;
        public ControllerReference Target => target as ControllerReference;

        IWorldReference _reference;
        World _world;

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
