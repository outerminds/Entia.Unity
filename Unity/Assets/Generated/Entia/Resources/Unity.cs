using Entia.Unity.Generation;

namespace Entia.Resources.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Resources.Unity), Link = "", Path = new string[] { "Entia", "Resources", "Unity" })][global::UnityEngine.AddComponentMenu("Entia/Resources/Entia.Resources.Unity")]
	public sealed partial class Unity : global::Entia.Unity.ResourceReference<global::Entia.Resources.Unity>
	{
		public global::UnityEngine.SceneManagement.Scene Scene
		{
			get => base.Get((ref global::Entia.Resources.Unity data, global::Entia.World world) => data.Scene, this._Scene);
			set => base.Set((ref global::Entia.Resources.Unity data, global::UnityEngine.SceneManagement.Scene state, global::Entia.World _) => data.Scene = state, value, ref this._Scene);
		}
		public global::Entia.Unity.WorldReference Reference
		{
			get => base.Get((ref global::Entia.Resources.Unity data, global::Entia.World world) => data.Reference, this._Reference);
			set => base.Set((ref global::Entia.Resources.Unity data, global::Entia.Unity.WorldReference state, global::Entia.World _) => data.Reference = state, value, ref this._Reference);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Scene))]
		global::UnityEngine.SceneManagement.Scene _Scene;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Reference))]
		global::Entia.Unity.WorldReference _Reference;
		public override global::Entia.Resources.Unity Raw
		{
			get => new global::Entia.Resources.Unity
			{
				Scene = this._Scene,
				Reference = this._Reference
			};
			set
			{
				this._Scene = value.Scene;
				this._Reference = value.Reference;
			}
		}

	}
}