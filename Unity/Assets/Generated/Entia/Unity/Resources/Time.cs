using Entia.Unity.Generation;

namespace Entia.Unity.Resources.Generated
{
	using System.Linq;

	[global::Entia.Unity.GeneratedAttribute(Type = typeof(global::Entia.Unity.Resources.Time), Link = "", Path = new string[] { "Entia", "Unity", "Resources", "Time" })]
	[global::UnityEngine.AddComponentMenu("Entia/Unity/Resources/Entia.Unity.Resources.Time")]
	public sealed partial class Time : global::Entia.Unity.ResourceReference<global::Entia.Unity.Resources.Time>
	{
		public ref global::System.Single Delta => ref this._Delta;
		public ref global::System.Single Current => ref this._Current;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Delta))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Delta;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Current))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Current;
		public override global::Entia.Unity.Resources.Time Resource
		{
			get => new global::Entia.Unity.Resources.Time
			{
				Delta = this.Delta,
				Current = this.Current
			};
			set
			{
				this.Delta = value.Delta;
				this.Current = value.Current;
			}
		}
	}
}