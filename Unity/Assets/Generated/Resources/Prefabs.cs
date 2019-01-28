using Entia.Unity.Generation;

namespace Resources.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Resources.Prefabs), Link = "Assets/Scripts/TestResources.cs", Path = new string[] { "Resources", "Prefabs" })]
	[global::UnityEngine.AddComponentMenu("Resources/Resources.Prefabs")]
	public sealed partial class Prefabs : global::Entia.Unity.ResourceReference<global::Resources.Prefabs>
	{
		public ref global::Entia.Unity.EntityReference Prefab => ref this._Prefab;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Prefab))]
		global::Entia.Unity.EntityReference _Prefab;
		public override global::Resources.Prefabs Resource
		{
			get => new global::Resources.Prefabs
			{
				Prefab = this.Prefab
			};
			set
			{
				this.Prefab = value.Prefab;
			}
		}
	}
}