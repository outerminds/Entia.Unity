using System;
using System.IO;
using Entia.Core;
using Entia.Unity;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class TemplateMenu
    {
        sealed class OnAssetNamed : EndNameEditAction
        {
            public override void Action(int instanceId, string pathName, string resourceFile)
            {
                var name = pathName.File(false);
                var format = resourceFile.Replace("#NAME#", name);
                var path = pathName.Absolute();
                File.WriteAllText(path, format);
                AssetDatabase.ImportAsset(pathName);
                var script = AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
                ProjectWindowUtil.ShowCreatedAsset(script);
            }
        }

        static void CreateScript(string name, string template)
        {
            var directory =
                Selection.activeObject == null ? Application.dataPath.Directory() :
                AssetDatabase.GetAssetPath(Selection.activeObject).Directory();
            var path = Path.Combine(directory, name).ChangeExtension(".cs");
            var texture = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<OnAssetNamed>(), path, texture, template);
        }

        [MenuItem("Assets/Create/Entia/System", priority = 50)]
        static void CreateSystem() => CreateScript("System",
@"using Entia;
using Entia.Core;
using Entia.Systems;
using Entia.Injectables;
using Entia.Queryables;
using Entia.Modules;
using Entia.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems
{
	public struct #NAME# : IRun
	{
		[Default]
		static #NAME# Default => new #NAME# { };

		void IRun.Run()
		{

		}
	}
}");

        [MenuItem("Assets/Create/Entia/Resource", priority = 51)]
        static void CreateResource() => CreateScript("Resource",
@"using Entia;
using Entia.Core;
using Entia.Unity;
using System.Collections;
using System.Collections.Generic;

namespace Resources
{
	public struct #NAME# : IResource
	{
		[Default]
		static #NAME# Default => new #NAME# { };
	}
}");

        [MenuItem("Assets/Create/Entia/Component", priority = 52)]
        static void CreateComponent() => CreateScript("Component",
@"using Entia;
using Entia.Core;
using Entia.Unity;
using Entia.Queryables;
using System.Collections;
using System.Collections.Generic;

namespace Components
{
	public struct #NAME# : IComponent
	{
		[Default]
		static #NAME# Default => new #NAME# { };
	}
}");

        [MenuItem("Assets/Create/Entia/Message", priority = 53)]
        static void CreateMessage() => CreateScript("Message",
@"using Entia;
using Entia.Core;
using Entia.Unity;
using System.Collections;
using System.Collections.Generic;

namespace Messages
{
	public struct #NAME# : IMessage
	{

	}
}");

        [MenuItem("Assets/Create/Entia/Controller", priority = 54)]
        static void CreateController() => CreateScript("Controller",
@"using Entia;
using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;
using System.Collections;
using System.Collections.Generic;

namespace Controllers
{
	public sealed class #NAME# : ControllerReference
	{
		public override Node Node =>
			Sequence(nameof(#NAME#),
				Nodes.Default
				// Insert systems here using System<T>() where 'T' is your system type.
			);
	}
}");
    }
}