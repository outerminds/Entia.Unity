﻿using Entia.Build;
using Entia.Core;
using Entia.Messages;
using Entia.Modules;
using Entia.Modules.Message;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(WorldReference), isFallback = true)]
    public class WorldReferenceEditor : UnityEditor.Editor
    {
        static bool _all;
        static bool _details;

        public WorldReference Target => target as WorldReference;

        readonly Dictionary<IRunner, TimeSpan> _elapsed = new Dictionary<IRunner, TimeSpan>();
        ReorderableList _modifiers;
        Receiver<OnProfile> _onProfile;

        protected virtual void OnEnable() => ReferenceUtility.Update();
        protected virtual void OnDestroy() => ReferenceUtility.Update();

        public override void OnInspectorGUI()
        {
            using (ReferenceUtility.World(serializedObject, Target, ref _modifiers))
            {
                if (Target.World is World world)
                {
                    UpdateProfile(world);
                    ShowModules(world);
                }
            }
        }

        void UpdateProfile(World world)
        {
            var messages = world.Messages();
            if (_onProfile != null)
            {
                while (_onProfile.TryPop(out var message))
                {
                    _elapsed[message.Runner] = message.Elapsed;
                    Repaint();
                }
                messages.Remove(_onProfile);
            }
            _onProfile = messages.Receiver<OnProfile>();
        }

        void ShowModules(World world)
        {
            using (LayoutUtility.Horizontal())
            {
                EditorGUILayout.LabelField(nameof(Modules), LayoutUtility.BoldLabel);
                _all = LayoutUtility.Toggle("All", _all);
            }
            using (LayoutUtility.Indent()) foreach (var module in world) ShowModule(module, world);
        }

        void ShowModule(IModule module, World world)
        {
            var label = module.GetType().Name;
            switch (module)
            {
                case Modules.Entities entities: world.ShowEntities(label, entities, label); break;
                case Modules.Components components: world.ShowComponents(label, world.Entities(), label); break;
                case Modules.Resources resources: world.ShowResources(label, resources, label); break;
                case Modules.Messages messages: world.ShowEmitters(label, messages, label); break;
                case Modules.Groups groups: world.ShowGroups(groups, label); break;
                case Modules.Families families: world.ShowFamilies(label, families.Roots(), label); break;
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
