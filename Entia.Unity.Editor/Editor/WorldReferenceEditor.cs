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
        static bool _details;

        public WorldReference Target => target as WorldReference;

        readonly Dictionary<IRunner, TimeSpan> _elapsed = new Dictionary<IRunner, TimeSpan>();
        Receiver<OnProfile> _onProfile;

        public override void OnInspectorGUI()
        {
            using (LayoutUtility.Disable(EditorApplication.isPlayingOrWillChangePlaymode)) base.OnInspectorGUI();
            if (Target.World is World world) ShowModules(world);
        }

        void OnEnable()
        {
            if (Target.World is World world)
            {
                _onProfile = world.Messages().Receiver<OnProfile>();
                EditorApplication.update += Update;
            }
        }

        void OnDisable()
        {
            if (Target.World is World world)
            {
                EditorApplication.update -= Update;
                world.Messages().Remove(_onProfile);
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

        void ShowModules(World world)
        {
            using (LayoutUtility.Horizontal())
            {
                EditorGUILayout.LabelField(nameof(Modules), LayoutUtility.BoldLabel);
                _all = LayoutUtility.Toggle("All", _all);
            }
            using (LayoutUtility.Indent()) foreach (var module in world) ShowModule(module, world);
            Repaint();
        }

        void ShowModule(IModule module, World world)
        {
            var label = module.GetType().Name;
            switch (module)
            {
                case Modules.Entities entities: world.ShowEntities(label, entities, label); break;
                case Modules.Components components: world.ShowComponents(label, world.Entities(), label); break;
                case Modules.Resources resources: world.ShowResources(label, world.Resources(), label); break;
                case Modules.Messages messages: world.ShowEmitters(label, messages, label); break;
                case Modules.Groups groups: world.ShowGroups(groups); break;
                case Modules.Controllers controllers: ShowControllers(controllers, world); break;
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

        void ShowControllers(Modules.Controllers module, World world) =>
            LayoutUtility.ChunksFoldout(
                nameof(Modules.Controllers),
                module.ToArray(),
                (controller, index) => world.ShowController(controller, _elapsed, _details, nameof(Modules.Controllers), index.ToString()),
                module.GetType(),
                foldout: data =>
                {
                    using (LayoutUtility.Horizontal())
                    {
                        var expanded = LayoutUtility.Foldout(data.label, data.type, data.path);
                        _details = LayoutUtility.Toggle("Details", _details);
                        return expanded;
                    }
                });
    }
}
