using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Physics), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Physics" })][global::UnityEngine.AddComponentMenu("Components/Components.Physics")]
	public sealed partial class Physics : global::Entia.Unity.ComponentReference<global::Components.Physics>
	{
		public ref global::System.Single Mass => ref base.Get((ref global::Components.Physics data) => ref data.Mass, ref this._Mass);
		public ref global::System.Single Drag => ref base.Get((ref global::Components.Physics data) => ref data.Drag, ref this._Drag);
		public ref global::System.Single Gravity => ref base.Get((ref global::Components.Physics data) => ref data.Gravity, ref this._Gravity);
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