using Entia.Unity.Generation;

namespace Resources.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Resources.Prefabs), Link = "Assets/Scripts/TestResources.cs", Path = new string[] { "Resources", "Prefabs" })][global::UnityEngine.AddComponentMenu("Resources/Resources.Prefabs")]
	public sealed partial class Prefabs : global::Entia.Unity.ResourceReference<global::Resources.Prefabs>
	{
		public global::Entia.Unity.EntityReference Prefab
		{
			get => base.Get((ref global::Resources.Prefabs data, global::Entia.World world) => data.Prefab, this._Prefab);
			set => base.Set((ref global::Resources.Prefabs data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.Prefab = state, value, ref this._Prefab);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Prefab))]
		global::Entia.Unity.EntityReference _Prefab;
		public override global::Resources.Prefabs Raw
		{
			get => new global::Resources.Prefabs
			{
				Prefab = this._Prefab
			};
			set
			{
				this._Prefab = value.Prefab;
			}
		}

	}
}