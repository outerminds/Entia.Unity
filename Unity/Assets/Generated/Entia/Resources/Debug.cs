using Entia.Unity.Generation;

namespace Entia.Resources.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Resources.Debug), Link = "", Path = new string[] { "Entia", "Resources", "Debug" })][global::UnityEngine.AddComponentMenu("Entia/Resources/Entia.Resources.Debug")]
	public sealed partial class Debug : global::Entia.Unity.ResourceReference<global::Entia.Resources.Debug>
	{
		public ref global::System.String Name => ref base.Get((ref global::Entia.Resources.Debug data) => ref data.Name, ref this._Name);
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Name))]
		global::System.String _Name;
		public override global::Entia.Resources.Debug Raw
		{
			get => new global::Entia.Resources.Debug
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