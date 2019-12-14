using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Velocity), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Velocity" })][global::UnityEngine.AddComponentMenu("Components/Components.Velocity")]
	public sealed partial class Velocity : global::Entia.Unity.ComponentReference<global::Components.Velocity>
	{
		public global::System.Single X
		{
			get => base.Get((ref global::Components.Velocity data, global::Entia.World world) => data.X, this._X);
			set => base.Set((ref global::Components.Velocity data, global::System.Single state, global::Entia.World _) => data.X = state, value, ref this._X);
		}
		public global::System.Single Y
		{
			get => base.Get((ref global::Components.Velocity data, global::Entia.World world) => data.Y, this._Y);
			set => base.Set((ref global::Components.Velocity data, global::System.Single state, global::Entia.World _) => data.Y = state, value, ref this._Y);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))]
		global::System.Single _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Y))]
		global::System.Single _Y;
		public override global::Components.Velocity Raw
		{
			get => new global::Components.Velocity
			{
				X = this._X,
				Y = this._Y
			};
			set
			{
				this._X = value.X;
				this._Y = value.Y;
			}
		}

	}
}