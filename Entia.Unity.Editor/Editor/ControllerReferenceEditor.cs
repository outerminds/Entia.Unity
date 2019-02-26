using System.Collections.Generic;
using Entia.Core;
using Entia.Dependencies;
using Entia.Modules;
using Entia.Modules.Build;
using Entia.Nodes;
using UnityEditor;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(ControllerReference), true, isFallback = true)]
    public class ControllerReferenceEditor : UnityEditor.Editor
    {
        static bool _details;

        public IWorldReference Reference => _reference ?? (_reference = Target.GetComponent<IWorldReference>());
        public World World => Target?.Controller?.World ?? Reference?.World ?? _world ?? (_world = Reference?.Create());
        public Node Node => _node ?? (_node = Target?.Node);
        public ControllerReference Target => target as ControllerReference;

        IWorldReference _reference;
        World _world;
        Node _node;
        readonly Dictionary<Node, Result<(IDependency[] dependencies, IRunner runner)>> _cache = new Dictionary<Node, Result<(IDependency[] dependencies, IRunner runner)>>();

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
                using (LayoutUtility.Indent()) world?.ShowNode(Node, _cache, _details);
            }
        }
    }
}
