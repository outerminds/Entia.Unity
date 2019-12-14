using Entia.Core;
using Entia.Unity.Generation;

namespace Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Jango), Link = "Assets/Jango.cs", Path = new string[] { "Jango" })][global::UnityEngine.AddComponentMenu("Jango")]
	public sealed partial class Jango : global::Entia.Unity.ComponentReference<global::Jango>
	{
		public global::Fett Fett
		{
			get => base.Get((ref global::Jango data, global::Entia.World world) => data.Fett, this._Fett);
			set => base.Set((ref global::Jango data, global::Fett state, global::Entia.World _) => data.Fett = state, value, ref this._Fett);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Fett))]
		global::Fett _Fett;
		public override global::Jango Raw
		{
			get => new global::Jango
			{
				Fett = this._Fett
			};
			set
			{
				this._Fett = value.Fett;
			}
		}

	}
}