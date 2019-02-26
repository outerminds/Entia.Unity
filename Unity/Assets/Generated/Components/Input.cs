using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Input), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Input" })][global::UnityEngine.AddComponentMenu("Components/Components.Input")]
	public sealed partial class Input : global::Entia.Unity.ComponentReference<global::Components.Input>
	{
		public ref global::System.Single Direction => ref base.Get((ref global::Components.Input data) => ref data.Direction, ref this._Direction);
		public ref global::System.Boolean Jump => ref base.Get((ref global::Components.Input data) => ref data.Jump, ref this._Jump);
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Direction))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Direction;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Jump))] [global::Entia.Unity.DisableAttribute()]
		global::System.Boolean _Jump;
		protected override global::Components.Input Raw
		{
			get => new global::Components.Input
			{
				Direction = this._Direction,
				Jump = this._Jump
			};
			set
			{
				this._Direction = value.Direction;
				this._Jump = value.Jump;
			}
		}

	}
}