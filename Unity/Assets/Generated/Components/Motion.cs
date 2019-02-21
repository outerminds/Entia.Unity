using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Motion), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Motion" })][global::UnityEngine.AddComponentMenu("Components/Components.Motion")]
	public sealed partial class Motion : global::Entia.Unity.ComponentReference<global::Components.Motion>
	{
		public ref global::System.Single Acceleration => ref base.Get((ref global::Components.Motion data) => ref data.Acceleration, ref this._Acceleration);
		public ref global::System.Single MaximumSpeed => ref base.Get((ref global::Components.Motion data) => ref data.MaximumSpeed, ref this._MaximumSpeed);
		public ref global::System.Single JumpForce => ref base.Get((ref global::Components.Motion data) => ref data.JumpForce, ref this._JumpForce);
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Acceleration))]
		global::System.Single _Acceleration;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(MaximumSpeed))]
		global::System.Single _MaximumSpeed;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(JumpForce))]
		global::System.Single _JumpForce;
		protected override global::Components.Motion Raw
		{
			get => new global::Components.Motion
			{
				Acceleration = this._Acceleration,
				MaximumSpeed = this._MaximumSpeed,
				JumpForce = this._JumpForce
			};
			set
			{
				this._Acceleration = value.Acceleration;
				this._MaximumSpeed = value.MaximumSpeed;
				this._JumpForce = value.JumpForce;
			}
		}

	}
}