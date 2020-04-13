using Entia.Build;
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

        protected virtual void OnDestroy() => ReferenceUtility.Initialize();

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
                while (_onProfile.TryMessage(out var message))
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
            using (Layout.Horizontal())
            {
                EditorGUILayout.LabelField(nameof(Modules), Layout.BoldLabel);
                _all = Layout.Toggle("All", _all);
            }
            using (Layout.Indent()) foreach (var module in world) ShowModule(module, world);
        }

        void ShowModule(IModule module, World world)
        {
            var label = module.GetType().Format();
            var path = new[] { label };
            switch (module)
            {
                case Modules.Entities entities: world.ShowEntities(label, entities, path); break;
                case Modules.Components components: world.ShowComponents(label, world.Entities(), path); break;
                case Modules.Resources resources: world.ShowResources(label, resources, path); break;
                case Modules.Messages messages: world.ShowEmitters(label, messages, path); break;
                case Modules.Groups groups: world.ShowGroups(groups, path); break;
                case Modules.Families families: world.ShowFamilies(label, families.Roots(), path); break;
                case Modules.Controllers controllers: ShowControllers(controllers, world); break;
                default:
                    if (_all)
                    {
                        using (Layout.Disable())
                        {
                            switch (module)
                            {
                                case Boxes boxes:
                                    Layout.ChunksFoldout(
                                        label,
                                        boxes.ToArray(),
                                        (item, index) => Layout.Members(
                                            $"{{ Type: {item.type.Format()}, Key: {item.key?.GetType().Format() ?? "null"}, Value: {item.value?.GetType().Format() ?? "null"} }}",
                                            new { Key = item.key, Value = item.value },
                                            item.type,
                                            path.Append(index.ToString())),
                                        module.GetType(),
                                        path);
                                    break;
                                case IEnumerable enumerable:
                                    var array = enumerable.Cast<object>().ToArray();
                                    Layout.ChunksFoldout(
                                        label,
                                        array,
                                        (item, _) => Layout.Label(item.GetType().FullFormat()),
                                        module.GetType(),
                                        path);
                                    break;
                                default:
                                    Layout.Label(module.GetType().Format());
                                    break;
                            }
                        }
                    }
                    break;
            }
        }

        void ShowControllers(Modules.Controllers module, World world) =>
            Layout.ChunksFoldout(
                nameof(Modules.Controllers),
                module.ToArray(),
                (controller, index) => world.ShowController(controller, _elapsed, _details, nameof(Modules.Controllers), index.ToString()),
                module.GetType(),
                foldout: data =>
                {
                    using (Layout.Horizontal())
                    {
                        var expanded = Layout.Foldout(data.label, data.type, data.path);
                        _details = Layout.Toggle("Details", _details);
                        return expanded;
                    }
                });
    }
}
