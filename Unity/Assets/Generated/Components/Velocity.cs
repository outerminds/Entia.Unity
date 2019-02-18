using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Velocity), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Velocity" })][global::UnityEngine.AddComponentMenu("Components/Components.Velocity")]
	public sealed partial class Velocity : global::Entia.Unity.ComponentReference<global::Components.Velocity>
	{
		ref global::System.Single X => ref this._X;
		ref global::System.Single Y => ref this._Y;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))]
		global::System.Single _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Y))]
		global::System.Single _Y;
		public override global::Components.Velocity Raw
		{
			get => new global::Components.Velocity
			{
				X = this.X,
				Y = this.Y
			};
			set
			{
				this.X = value.X;
				this.Y = value.Y;
			}
		}

	}
}