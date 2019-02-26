using Entia.Unity.Generation;

namespace Entia.Resources.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Resources.Time), Link = "", Path = new string[] { "Entia", "Resources", "Time" })][global::UnityEngine.AddComponentMenu("Entia/Resources/Entia.Resources.Time")]
	public sealed partial class Time : global::Entia.Unity.ResourceReference<global::Entia.Resources.Time>
	{
		public ref global::System.Single Delta => ref base.Get((ref global::Entia.Resources.Time data) => ref data.Delta, ref this._Delta);
		public ref global::System.Single Current => ref base.Get((ref global::Entia.Resources.Time data) => ref data.Current, ref this._Current);
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Delta))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Delta;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Current))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Current;
		protected override global::Entia.Resources.Time Raw
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