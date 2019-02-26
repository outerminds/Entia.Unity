using Entia.Unity.Generation;

namespace Resources.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Resources.Prefabs), Link = "Assets/Scripts/TestResources.cs", Path = new string[] { "Resources", "Prefabs" })][global::UnityEngine.AddComponentMenu("Resources/Resources.Prefabs")]
	public sealed partial class Prefabs : global::Entia.Unity.ResourceReference<global::Resources.Prefabs>
	{
		public ref global::Entia.Unity.EntityReference Prefab => ref base.Get((ref global::Resources.Prefabs data) => ref data.Prefab, ref this._Prefab);
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Prefab))]
		global::Entia.Unity.EntityReference _Prefab;
		protected override global::Resources.Prefabs Raw
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