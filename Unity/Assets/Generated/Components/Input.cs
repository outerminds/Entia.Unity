using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Input), Link = "Assets/Scripts/Example/Components.cs", Path = new string[] { "Components", "Input" })][global::UnityEngine.AddComponentMenu("Components/Components.Input")]
	public sealed partial class Input : global::Entia.Unity.ComponentReference<global::Components.Input>
	{
		public global::System.Single Direction
		{
			get => base.Get((ref global::Components.Input data, global::Entia.World world) => data.Direction, this._Direction);
			set => base.Set((ref global::Components.Input data, global::System.Single state, global::Entia.World _) => data.Direction = state, value, ref this._Direction);
		}
		public global::System.Boolean Jump
		{
			get => base.Get((ref global::Components.Input data, global::Entia.World world) => data.Jump, this._Jump);
			set => base.Set((ref global::Components.Input data, global::System.Boolean state, global::Entia.World _) => data.Jump = state, value, ref this._Jump);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Direction))] [global::Entia.Unity.DisableAttribute()]
		global::System.Single _Direction;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Jump))] [global::Entia.Unity.DisableAttribute()]
		global::System.Boolean _Jump;
		public override global::Components.Input Raw
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