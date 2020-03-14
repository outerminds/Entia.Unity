// using Entia.Core;
// using Entia.Experimental.Templating;
// using Entia.Modules;
// using Entia.Unity.Editor;
// using UnityEditor;
// using UnityEngine;

// namespace Entia.Experimental.Unity.Editor
// {
//     struct Parent : IComponent { public Entity Value; }
//     struct Holder : IComponent { public UnityEngine.Object Value; }

//     [CustomEditor(typeof(TemplateReference), isFallback = true)]
//     public class TemplateReferenceEditor : UnityEditor.Editor
//     {
//         public TemplateReference Target => target as TemplateReference;

//         public override void OnInspectorGUI()
//         {
//             base.OnInspectorGUI();

//             var world = new World();
//             var entity = world.Instantiate(Target.Template);
//             if (entity == Entity.Zero) entity = world.Entities().Create();

//             LayoutUtility.Object("Template", Target.Template, Target.Template.GetType(), "Template");

//             EditorGUI.BeginChangeCheck();
//             LayoutUtility.Label(entity.ToString());
//             ShowEntity(entity, world);
//             if (EditorGUI.EndChangeCheck())
//                 Target.Template = world.Template(entity);
//         }

//         private void ShowEntity(Entity entity, World world)
//         {
//             var entities = world.Entities();
//             var components = world.Components();
//             var families = world.Families();

//             using (LayoutUtility.Horizontal())
//             {
//                 using (LayoutUtility.Vertical())
//                 {
//                     if (LayoutUtility.Foldout("Children", typeof(Entity[]), "Children"))
//                     {
//                         using (LayoutUtility.Indent())
//                         {
//                             var children = families.Children(entity);
//                             for (int i = 0; i < children.Count; i++)
//                             {
//                                 var child = children[i];
//                                 using (LayoutUtility.Horizontal())
//                                 {
//                                     using (LayoutUtility.Vertical())
//                                     {
//                                         if (LayoutUtility.Foldout(child.ToString(world), typeof(Entity), i.ToString()))
//                                         {
//                                             using (LayoutUtility.Indent())
//                                                 ShowEntity(child, world);
//                                         }
//                                     }

//                                     if (LayoutUtility.MinusButton()) families.Reject(child);
//                                 }
//                             }
//                         }
//                     }
//                 }

//                 if (LayoutUtility.PlusButton()) families.Adopt(entity, entities.Create());
//             }

//             if (LayoutUtility.Foldout("Components", typeof(Entity[]), "Components"))
//             {
//                 using (LayoutUtility.Indent())
//                 {
//                     foreach (var component in components.Get(entity))
//                     {
//                         var type = component.GetType();
//                         using (LayoutUtility.Horizontal())
//                         {
//                             using (LayoutUtility.Vertical())
//                             {
//                                 var modified = LayoutUtility.Object(type.FullFormat(), component, type, nameof(TemplateReferenceEditor), type.FullName);
//                                 if (modified is IComponent casted) components.Set(entity, casted);
//                             }
//                             if (LayoutUtility.MinusButton()) components.Remove(entity, type);
//                         }
//                     }

//                     if (GUILayout.Button("Add Parent"))
//                         components.Set(entity, new Parent { Value = families.Parent(entity) });
//                     if (GUILayout.Button("Add Holder"))
//                         components.Set(entity, new Holder { Value = default });
//                     if (GUILayout.Button("Add Debug"))
//                         components.Set(entity, new Components.Debug { Name = Target.name });
//                     if (GUILayout.Button("Add Transform"))
//                         components.Set(entity, new Components.Unity<Transform> { Value = Target.transform });
//                     if (GUILayout.Button("Add GameObject"))
//                         components.Set(entity, new Components.Unity<GameObject> { Value = Target.gameObject });
//                 }
//             }
//         }
//     }
// }
