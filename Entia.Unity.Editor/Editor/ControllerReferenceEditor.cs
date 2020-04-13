﻿using System.Collections.Generic;
using Entia.Build;
using Entia.Core;
using Entia.Dependencies;
using Entia.Nodes;
using UnityEditor;
using UnityEditorInternal;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(ControllerReference), true, isFallback = true)]
    public class ControllerReferenceEditor : UnityEditor.Editor
    {
        static bool _details;

        public IWorldReference Reference => _reference ?? (_reference = Target.GetComponent<IWorldReference>());
        public World World => Target?.World ?? Reference?.World ?? _world ?? (_world = Reference?.Create());
        public ControllerReference Target => target as ControllerReference;

        IWorldReference _reference;
        World _world;
        ReorderableList _nodes;
        readonly Dictionary<Node, Result<(IDependency[] dependencies, IRunner runner)>> _cache = new Dictionary<Node, Result<(IDependency[] dependencies, IRunner runner)>>();

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            using (ReferenceUtility.Controller(serializedObject, Target, ref _nodes)) ShowNode();
            if (EditorGUI.EndChangeCheck()) Repaint();
        }

        void ShowNode()
        {
            if (World is World world && Target?.Create() is Node node)
            {
                using (Layout.Horizontal())
                {
                    EditorGUILayout.LabelField(nameof(Node), Layout.BoldLabel);
                    _details = Layout.Toggle("Details", _details);
                }
                using (Layout.Indent()) world.ShowNode(node, true, _cache, _details, nameof(ControllerReferenceEditor));
            }
        }
    }
}
