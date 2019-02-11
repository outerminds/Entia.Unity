using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Motion), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Motion" })]
	[global::UnityEngine.AddComponentMenu("Components/Components.Motion")]
	public sealed partial class Motion : global::Entia.Unity.ComponentReference<global::Components.Motion>
	{
		public ref global::System.Single Acceleration => ref this._Acceleration;
		public ref global::System.Single MaximumSpeed => ref this._MaximumSpeed;
		public ref global::System.Single JumpForce => ref this._JumpForce;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Acceleration))] [global::Entia.Unity.DefaultAttribute(2f)]
		global::System.Single _Acceleration = 2f;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(MaximumSpeed))] [global::Entia.Unity.DefaultAttribute(0.25f)]
		global::System.Single _MaximumSpeed = 0.25f;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(JumpForce))] [global::Entia.Unity.DefaultAttribute(0.75f)]
		global::System.Single _JumpForce = 0.75f;
		public override global::Components.Motion Component
		{
			get => new global::Components.Motion
			{
				Acceleration = this.Acceleration,
				MaximumSpeed = this.MaximumSpeed,
				JumpForce = this.JumpForce
			};
			set
			{
				this.Acceleration = value.Acceleration;
				this.MaximumSpeed = value.MaximumSpeed;
				this.JumpForce = value.JumpForce;
			}
		}
	}
}