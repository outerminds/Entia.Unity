using Entia.Unity.Generation;

namespace Entia.Resources.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Resources.Time), Link = "", Path = new string[] { "Entia", "Resources", "Time" })][global::UnityEngine.AddComponentMenu("Entia/Resources/Entia.Resources.Time")]
	public sealed partial class Time : global::Entia.Unity.ResourceReference<global::Entia.Resources.Time>
	{
		public global::System.Single Delta
		{
			get => base.Get((ref global::Entia.Resources.Time data, global::Entia.World world) => data.Delta, this._Delta);
			set => base.Set((ref global::Entia.Resources.Time data, global::System.Single state, global::Entia.World _) => data.Delta = state, value, ref this._Delta);
		}
		public global::System.Single Current
		{
			get => base.Get((ref global::Entia.Resources.Time data, global::Entia.World world) => data.Current, this._Current);
			set => base.Set((ref global::Entia.Resources.Time data, global::System.Single state, global::Entia.World _) => data.Current = state, value, ref this._Current);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Delta))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Delta;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Current))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Current;
		public override global::Entia.Resources.Time Raw
		{
			get => new global::Entia.Resources.Time
			{
				Delta = this._Delta,
				Current = this._Current
			};
			set
			{
				this._Delta = value.Delta;
				this._Current = value.Current;
			}
		}

	}
}