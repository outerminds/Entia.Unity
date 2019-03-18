using Entia.Core;
using Entia.Unity.Generation;

namespace Entia.Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Entia.Components.Pooled), Link = "Assets/Plugins/Entia.Pool/Pooled.cs", Path = new string[] { "Entia", "Components", "Pooled" })][global::UnityEngine.AddComponentMenu("Entia/Components/Entia.Components.Pooled")]
	public sealed partial class Pooled : global::Entia.Unity.ComponentReference<global::Entia.Components.Pooled>
	{
		public ref global::Entia.Unity.EntityReference Reference => ref base.Get((ref global::Entia.Components.Pooled data) => ref data.Reference, ref this._Reference);
		public ref global::UnityEngine.GameObject Instance => ref base.Get((ref global::Entia.Components.Pooled data) => ref data.Instance, ref this._Instance);
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Reference))]
		global::Entia.Unity.EntityReference _Reference;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Instance))]
		global::UnityEngine.GameObject _Instance;
		public override global::Entia.Components.Pooled Raw
		{
			get => new global::Entia.Components.Pooled
			{
				Reference = this._Reference,
				Instance = this._Instance
			};
			set
			{
				this._Reference = value.Reference;
				this._Instance = value.Instance;
			}
		}

	}
}