using System.IO;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class ScriptUtility
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
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(pathName);
                ProjectWindowUtil.ShowCreatedAsset(script);
            }
        }

        static void CreateScript(string name, string template)
        {
            var asset = Selection.activeObject == null ? "" : AssetDatabase.GetAssetPath(Selection.activeObject);
            var directory = string.IsNullOrWhiteSpace(asset) ? Application.dataPath.Directory() : asset.Directory();
            var path = Path.Combine(directory, name).ChangeExtension(".cs").Relative(Application.dataPath);
            var texture = EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D;
            var action = ScriptableObject.CreateInstance<OnAssetNamed>();
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, action, path, texture, template);
        }

        [MenuItem("Assets/Create/Entia/System", priority = 50)]
        public static void CreateSystem() => CreateScript("System",
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
        public static void CreateResource() => CreateScript("Resource",
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
        public static void CreateComponent() => CreateScript("Component",
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
        public static void CreateMessage() => CreateScript("Message",
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

        // [MenuItem("Assets/Create/Entia/Controller", priority = 54)]
        public static void CreateController() => CreateScript("Controller",
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

        [MenuItem("Assets/Create/Entia/Node", priority = 54)]
        public static void CreateNode() => CreateScript("Node",
@"using Entia;
using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;
using static Entia.Unity.Nodes;

namespace Nodes
{
	public sealed class #NAME# : NodeReference
	{
		public override Node Node => Sequence(nameof(#NAME#),
			Default
			// Insert systems here using System<T>() where 'T' is your system type.
		);
	}
}");

        [MenuItem("Assets/Create/Entia/Modifier", priority = 55)]
        public static void CreateModifier() => CreateScript("Modifier",
@"using Entia;
using Entia.Core;
using Entia.Modules;
using Entia.Unity;

namespace Modifiers
{
	public sealed class #NAME# : WorldModifier
	{
		public override void Modify(World world)
		{
		}
	}
}");
    }
}