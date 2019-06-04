using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Link), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Link" })][global::UnityEngine.AddComponentMenu("Components/Components.Link")]
	public sealed partial class Link : global::Entia.Unity.ComponentReference<global::Components.Link>
	{
		public global::Entia.Unity.EntityReference HasComponent
		{
			get => base.Get((ref global::Components.Link data, global::Entia.World world) => data.HasComponent.FromEntia(world), this._HasComponent);
			set => base.Set((ref global::Components.Link data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.HasComponent = state.ToEntia(), value, ref this._HasComponent);
		}
		public global::Entia.Unity.EntityReference NoComponent
		{
			get => base.Get((ref global::Components.Link data, global::Entia.World world) => data.NoComponent.FromEntia(world), this._NoComponent);
			set => base.Set((ref global::Components.Link data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.NoComponent = state.ToEntia(), value, ref this._NoComponent);
		}
		public global::Entia.Unity.EntityReference HasUnity
		{
			get => base.Get((ref global::Components.Link data, global::Entia.World world) => data.HasUnity.FromEntia(world), this._HasUnity);
			set => base.Set((ref global::Components.Link data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.HasUnity = state.ToEntia(), value, ref this._HasUnity);
		}
		public global::Entia.Unity.EntityReference NoUnity
		{
			get => base.Get((ref global::Components.Link data, global::Entia.World world) => data.NoUnity.FromEntia(world), this._NoUnity);
			set => base.Set((ref global::Components.Link data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.NoUnity = state.ToEntia(), value, ref this._NoUnity);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(HasComponent))] [global::Entia.Queryables.AllAttribute(new global::System.Type[] { typeof(Entia.IComponent) })]
		global::Entia.Unity.EntityReference _HasComponent;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(NoComponent))] [global::Entia.Queryables.NoneAttribute(new global::System.Type[] { typeof(Entia.IComponent) })]
		global::Entia.Unity.EntityReference _NoComponent;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(HasUnity))] [global::Entia.Queryables.AllAttribute(new global::System.Type[] { typeof(Entia.Components.Unity<>) })]
		global::Entia.Unity.EntityReference _HasUnity;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(NoUnity))] [global::Entia.Queryables.NoneAttribute(new global::System.Type[] { typeof(Entia.Components.Unity<>) })]
		global::Entia.Unity.EntityReference _NoUnity;
		public override global::Components.Link Raw
		{
			get => new global::Components.Link
			{
				HasComponent = this._HasComponent.ToEntia(),
				NoComponent = this._NoComponent.ToEntia(),
				HasUnity = this._HasUnity.ToEntia(),
				NoUnity = this._NoUnity.ToEntia()
			};
			set
			{
				this._HasComponent = value.HasComponent.FromEntia(base.World);
				this._NoComponent = value.NoComponent.FromEntia(base.World);
				this._HasUnity = value.HasUnity.FromEntia(base.World);
				this._NoUnity = value.NoUnity.FromEntia(base.World);
			}
		}

	}
}