using Entia.Core;
using Entia.Initializers;

namespace Entia.Unity.Initializers
{
	public sealed class GameObject : Initializer<UnityEngine.GameObject>
	{
		public readonly string Name;
		public readonly string Tag;
		public readonly int Layer;
		public readonly bool Active;
		public readonly int[] Children;

		public GameObject(string name, string tag, int layer, bool active, params int[] children)
		{
			Name = name;
			Tag = tag;
			Layer = layer;
			Active = active;
			Children = children;
		}

		public override Result<Unit> Initialize(UnityEngine.GameObject instance, object[] instances)
		{
			var parent = instance.transform;
			foreach (var index in Children)
			{
				var result = Result.Cast<UnityEngine.GameObject>(index);
				if (result.TryValue(out var child)) child.transform.SetParent(parent);
				else return result;
			}

			instance.name = Name;
			instance.tag = Tag;
			instance.layer = Layer;
			instance.SetActive(Active);
			return Result.Success();
		}
	}
}
