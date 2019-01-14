using Entia.Unity.Generation;

namespace Entia.Unity.Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.GeneratedAttribute(Type = typeof(global::Entia.Unity.Components.Debug), Link = "", Path = new string[] { "Entia", "Unity", "Components", "Debug" })]
	[global::UnityEngine.AddComponentMenu("Entia/Unity/Components/Entia.Unity.Components.Debug")]
	public sealed partial class Debug : global::Entia.Unity.ComponentReference<global::Entia.Unity.Components.Debug>
	{
		public ref global::System.String Name => ref this._Name;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Name))]
		global::System.String _Name;
		public override global::Entia.Unity.Components.Debug Component
		{
			get => new global::Entia.Unity.Components.Debug
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