using System;
using System.IO;
using Entia.Unity;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    public static class TemplateInstaller
    {
        static TemplateInstaller() { Install(false); }

        [MenuItem("Entia/Install/Templates")]
        public static void Install() => Install(true);

        public static void Install(bool log)
        {
            var directory = PathUtility.Replace("{Editor.Templates}");

            void Write(string name, int order, string template)
            {
                var path = Path.Combine(directory, $"{order}-Entia__{name}-New{name}.cs.txt");
                try { File.WriteAllText(path, template); }
                catch
                {
                    if (log)
                    {
                        Debug.LogWarning(
$@"Failed to install template '{name}' at path '{path}'.
This may be happening because Unity is not running in administrator mode.
-> Close the Unity editor and Unity hub.
-> Launch Unity hub in administrator mode.
-> Launch your project.
-> Retry to install the templates.");
                        throw;
                    }
                }
            }

            Write("System", 50,
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
	public struct #SCRIPTNAME# : IRun
	{
		[Default]
		static #SCRIPTNAME# Default => new #SCRIPTNAME# { };

		void IRun.Run()
		{

		}
	}
}");

            Write("Resource", 51,
@"using Entia;
using Entia.Core;
using Entia.Unity;
using System.Collections;
using System.Collections.Generic;

namespace Resources
{
	public struct #SCRIPTNAME# : IResource
	{
		
	}
}");

            Write("Component", 52,
@"using Entia;
using Entia.Core;
using Entia.Unity;
using Entia.Queryables;
using System.Collections;
using System.Collections.Generic;

namespace Components
{
	public struct #SCRIPTNAME# : IComponent
	{
		
	}
}");


            Write("Message", 54,
@"using Entia;
using Entia.Core;
using Entia.Unity;
using System.Collections;
using System.Collections.Generic;

namespace Messages
{
	public struct #SCRIPTNAME# : IMessage
	{
		
	}
}");

            Write("Controller", 55,
@"using Entia;
using Entia.Core;
using Entia.Nodes;
using Entia.Unity;
using static Entia.Nodes.Node;
using System.Collections;
using System.Collections.Generic;

namespace Controllers
{
	public sealed class #SCRIPTNAME# : ControllerReference
	{
		public override Node Node =>
			Sequence(nameof(#SCRIPTNAME#)),
				Nodes.Default
				// Insert systems here using System<T>() where 'T' is your system type.
			);
	}
}");

            if (log) Debug.Log(
$@"Templates installed to '{directory}'.
The Unity editor may need to be restarted for changes to take effect.");
        }
    }
}