using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Physics), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Physics" })][global::UnityEngine.AddComponentMenu("Components/Components.Physics")]
	public sealed partial class Physics : global::Entia.Unity.ComponentReference<global::Components.Physics>
	{
		ref global::System.Single Mass => ref this._Mass;
		ref global::System.Single Drag => ref this._Drag;
		ref global::System.Single Gravity => ref this._Gravity;
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