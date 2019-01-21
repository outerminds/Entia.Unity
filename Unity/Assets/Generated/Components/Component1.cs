using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Component1), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Component1" })]
	[global::UnityEngine.AddComponentMenu("Components/Components.Component1")]
	public sealed partial class Component1 : global::Entia.Unity.ComponentReference<global::Components.Component1>
	{
		public ref global::System.Single X => ref this._X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))] [global::UnityEngine.Serialization.FormerlySerializedAsAttribute("Poulah")]
		global::System.Single _X;
		public override global::Components.Component1 Component
		{
			get => new global::Components.Component1
			{
				X = this.X
			};
			set
			{
				this.X = value.X;
			}
		}
	}
}