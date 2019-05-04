using Entia.Unity.Generation;

namespace Entia.Resources.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Resources.Unity), Link = "", Path = new string[] { "Entia", "Resources", "Unity" })][global::UnityEngine.AddComponentMenu("Entia/Resources/Entia.Resources.Unity")]
	public sealed partial class Unity : global::Entia.Unity.ResourceReference<global::Entia.Resources.Unity>
	{
		public ref global::UnityEngine.SceneManagement.Scene Scene => ref base.Get((ref global::Entia.Resources.Unity data) => ref data.Scene, ref this._Scene);
		public ref global::Entia.Unity.WorldReference Reference => ref base.Get((ref global::Entia.Resources.Unity data) => ref data.Reference, ref this._Reference);
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