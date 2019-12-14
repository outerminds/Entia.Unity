using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Motion), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Motion" })][global::UnityEngine.AddComponentMenu("Components/Components.Motion")]
	public sealed partial class Motion : global::Entia.Unity.ComponentReference<global::Components.Motion>
	{
		public global::System.Single Acceleration
		{
			get => base.Get((ref global::Components.Motion data, global::Entia.World world) => data.Acceleration, this._Acceleration);
			set => base.Set((ref global::Components.Motion data, global::System.Single state, global::Entia.World _) => data.Acceleration = state, value, ref this._Acceleration);
		}
		public global::System.Single Speed
		{
			get => base.Get((ref global::Components.Motion data, global::Entia.World world) => data.Speed, this._Speed);
			set => base.Set((ref global::Components.Motion data, global::System.Single state, global::Entia.World _) => data.Speed = state, value, ref this._Speed);
		}
		public global::System.Single Jump
		{
			get => base.Get((ref global::Components.Motion data, global::Entia.World world) => data.Jump, this._Jump);
			set => base.Set((ref global::Components.Motion data, global::System.Single state, global::Entia.World _) => data.Jump = state, value, ref this._Jump);
		}
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