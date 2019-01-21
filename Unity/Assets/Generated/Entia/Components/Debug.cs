using Entia.Unity.Generation;

namespace Entia.Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Components.Debug), Link = "", Path = new string[] { "Entia", "Components", "Debug" })]
	[global::UnityEngine.AddComponentMenu("Entia/Components/Entia.Components.Debug")]
	public sealed partial class Debug : global::Entia.Unity.ComponentReference<global::Entia.Components.Debug>
	{
		public ref global::System.String Name => ref this._Name;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Name))]
		global::System.String _Name;
		public override global::Entia.Components.Debug Component
		{
			get => new global::Entia.Components.Debug
			{
				Name = this.Name
			};
			set
			{
				this.Name = value.Name;
			}
		}
	}
}