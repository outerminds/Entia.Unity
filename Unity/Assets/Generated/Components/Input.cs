using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Input), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Input" })]
	[global::UnityEngine.AddComponentMenu("Components/Components.Input")]
	public sealed partial class Input : global::Entia.Unity.ComponentReference<global::Components.Input>
	{
		public ref global::System.Single Direction => ref this._Direction;
		public ref global::System.Boolean Jump => ref this._Jump;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Direction))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Direction;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Jump))] [global::Entia.Unity.DisableAttribute()]
		global::System.Boolean _Jump;
		public override global::Components.Input Component
		{
			get => new global::Components.Input
			{
				Direction = this.Direction,
				Jump = this.Jump
			};
			set
			{
				this.Direction = value.Direction;
				this.Jump = value.Jump;
			}
		}
	}
}