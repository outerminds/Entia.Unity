using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Motion), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Motion" })][global::UnityEngine.AddComponentMenu("Components/Components.Motion")]
	public sealed partial class Motion : global::Entia.Unity.ComponentReference<global::Components.Motion>
	{
		ref global::System.Single Acceleration => ref this._Acceleration;
		ref global::System.Single MaximumSpeed => ref this._MaximumSpeed;
		ref global::System.Single JumpForce => ref this._JumpForce;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Acceleration))]
		global::System.Single _Acceleration;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(MaximumSpeed))]
		global::System.Single _MaximumSpeed;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(JumpForce))]
		global::System.Single _JumpForce;
		public override global::Components.Motion Raw
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