using Entia.Core;
using Entia.Unity.Generation;

namespace Entia.Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Components.Debug), Link = "", Path = new string[] { "Entia", "Components", "Debug" })][global::UnityEngine.AddComponentMenu("Entia/Components/Entia.Components.Debug")]
	public sealed partial class Debug : global::Entia.Unity.ComponentReference<global::Entia.Components.Debug>
	{
		public global::System.String Name
		{
			get => base.Get((ref global::Entia.Components.Debug data, global::Entia.World world) => data.Name, this._Name);
			set => base.Set((ref global::Entia.Components.Debug data, global::System.String state, global::Entia.World _) => data.Name = state, value, ref this._Name);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Name))]
		global::System.String _Name;
		public override global::Entia.Components.Debug Raw
		{
			get => new global::Entia.Components.Debug
			{
				Name = this._Name
			};
			set
			{
				this._Name = value.Name;
			}
		}

	}
}