using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Physics), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Physics" })][global::UnityEngine.AddComponentMenu("Components/Components.Physics")]
	public sealed partial class Physics : global::Entia.Unity.ComponentReference<global::Components.Physics>
	{
		public global::System.Single Mass
		{
			get => base.Get((ref global::Components.Physics data, global::Entia.World world) => data.Mass, this._Mass);
			set => base.Set((ref global::Components.Physics data, global::System.Single state, global::Entia.World _) => data.Mass = state, value, ref this._Mass);
		}
		public global::System.Single Drag
		{
			get => base.Get((ref global::Components.Physics data, global::Entia.World world) => data.Drag, this._Drag);
			set => base.Set((ref global::Components.Physics data, global::System.Single state, global::Entia.World _) => data.Drag = state, value, ref this._Drag);
		}
		public global::System.Single Gravity
		{
			get => base.Get((ref global::Components.Physics data, global::Entia.World world) => data.Gravity, this._Gravity);
			set => base.Set((ref global::Components.Physics data, global::System.Single state, global::Entia.World _) => data.Gravity = state, value, ref this._Gravity);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Mass))]
		global::System.Single _Mass;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Drag))]
		global::System.Single _Drag;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Gravity))]
		global::System.Single _Gravity;
		public override global::Components.Physics Raw
		{
			get => new global::Components.Physics
			{
				Mass = this._Mass,
				Drag = this._Drag,
				Gravity = this._Gravity
			};
			set
			{
				this._Mass = value.Mass;
				this._Drag = value.Drag;
				this._Gravity = value.Gravity;
			}
		}

	}
}