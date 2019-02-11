using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Physics), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Physics" })]
	[global::UnityEngine.AddComponentMenu("Components/Components.Physics")]
	public sealed partial class Physics : global::Entia.Unity.ComponentReference<global::Components.Physics>
	{
		public ref global::System.Single Mass => ref this._Mass;
		public ref global::System.Single Drag => ref this._Drag;
		public ref global::System.Single Gravity => ref this._Gravity;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Mass))] [global::Entia.Unity.DefaultAttribute(1f)]
		global::System.Single _Mass = 1f;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Drag))] [global::Entia.Unity.DefaultAttribute(3f)]
		global::System.Single _Drag = 3f;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Gravity))] [global::Entia.Unity.DefaultAttribute(-2f)]
		global::System.Single _Gravity = -2f;
		public override global::Components.Physics Component
		{
			get => new global::Components.Physics
			{
				Mass = this.Mass,
				Drag = this.Drag,
				Gravity = this.Gravity
			};
			set
			{
				this.Mass = value.Mass;
				this.Drag = value.Drag;
				this.Gravity = value.Gravity;
			}
		}
	}
}