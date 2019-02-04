using Entia.Core;
using Entia.Messages;
using Entia.Modules;
using Entia.Modules.Build;
using Entia.Modules.Message;
using Entia.Nodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(WorldReference), isFallback = true)]
    public class WorldReferenceEditor : UnityEditor.Editor
    {
        static bool _all;

        public WorldReference Target => target as WorldReference;
        public World World => Target?.World;

        readonly Dictionary<IRunner, TimeSpan> _elapsed = new Dictionary<IRunner, TimeSpan>();
        Receiver<OnProfile> _onProfile;

        public override void OnInspectorGUI()
        {
            using (LayoutUtility.Disable(EditorApplication.isPlayingOrWillChangePlaymode)) base.OnInspectorGUI();
            if (World != null) ShowModules();
        }

        void OnEnable()
        {
            if (World != null)
            {
                _onProfile = World.Messages().Receiver<OnProfile>();
                EditorApplication.update += Update;
            }
        }

        void OnDisable()
        {
            if (World != null)
            {
                EditorApplication.update -= Update;
                World.Messages().Remove(_onProfile);
            }
        }

        void Update()
        {
            while (_onProfile.TryPop(out var message))
            {
                _elapsed[message.Runner] = message.Elapsed;
                Repaint();
            }
        }

        void ShowModules()
        {
            using (LayoutUtility.Horizontal())
            {
                EditorGUILayout.LabelField(nameof(Modules), LayoutUtility.BoldLabel);
                _all = LayoutUtility.Toggle("All", _all);
            }
            using (LayoutUtility.Indent()) foreach (var module in World) ShowModule(module);
            Repaint();
        }

        void ShowModule(IModule module)
        {
            var label = module.GetType().Name;
            switch (module)
            {
                case Modules.Entities entities: World.ShowEntities(label, entities, label); break;
                case Modules.Components components: World.ShowComponents(label, World.Entities(), label); break;
                case Modules.Resources resources: World.ShowResources(label, World.Resources(), label); break;
                case Modules.Messages messages: World.ShowEmitters(label, messages, label); break;
                case Modules.Groups groups: ShowGroups(groups); break;
                case Modules.Controllers controllers: ShowControllers(controllers); break;
                default:
                    if (_all)
                    {
                        using (LayoutUtility.Disable())
                        {
                            if (module is IEnumerable enumerable)
                            {
                                var array = enumerable.Cast<object>().ToArray();
                                var format = module.GetType().Format();
                                LayoutUtility.ChunksFoldout(
                                    format,
                                    array,
                                    (item, _) => LayoutUtility.Label(item.GetType().FullFormat()),
                                    module.GetType(),
                                    new[] { format });
                            }
                            else LayoutUtility.Label(module.GetType().Format());
                        }
                    }
                    break;
            }

            Repaint();
        }

        void ShowGroups(Modules.Groups module) =>
            LayoutUtility.ChunksFoldout(
                nameof(Modules.Groups),
                module.ToArray(),
                (group, index) => World.ShowGroup(group.Type.Format(), group, nameof(Modules.Groups), index.ToString()),
                module.GetType());

        void ShowControllers(Modules.Controllers module) =>
            LayoutUtility.ChunksFoldout(
                nameof(Modules.Controllers),
                module.ToArray(),
                (controller, index) => World.ShowController(controller, _elapsed, nameof(Modules.Controllers), index.ToString()),
                module.GetType());
    }
}
