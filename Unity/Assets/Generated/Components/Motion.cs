using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Motion), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Motion" })][global::UnityEngine.AddComponentMenu("Components/Components.Motion")]
	public sealed partial class Motion : global::Entia.Unity.ComponentReference<global::Components.Motion>
	{
		public ref global::System.Single Acceleration => ref base.Get((ref global::Components.Motion data) => ref data.Acceleration, ref this._Acceleration);
		public ref global::System.Single Speed => ref base.Get((ref global::Components.Motion data) => ref data.Speed, ref this._Speed);
		public ref global::System.Single Jump => ref base.Get((ref global::Components.Motion data) => ref data.Jump, ref this._Jump);
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Acceleration))]
		global::System.Single _Acceleration;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Speed))]
		global::System.Single _Speed;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Jump))]
		global::System.Single _Jump;
		public override global::Components.Motion Raw
		{
			get => new global::Components.Motion
			{
				Acceleration = this._Acceleration,
				Speed = this._Speed,
				Jump = this._Jump
			};
			set
			{
				this._Acceleration = value.Acceleration;
				this._Speed = value.Speed;
				this._Jump = value.Jump;
			}
		}

	}
}