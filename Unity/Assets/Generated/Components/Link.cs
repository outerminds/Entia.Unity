using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Link), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Link" })]
	[global::UnityEngine.AddComponentMenu("Components/Components.Link")]
	public sealed partial class Link : global::Entia.Unity.ComponentReference<global::Components.Link>
	{
		public global::Entia.Entity HasComponent
		{
			get => this._HasComponent.ToEntia();
			set => this._HasComponent = value.FromEntia(this.World);
		}
		public global::Entia.Entity NoComponent
		{
			get => this._NoComponent.ToEntia();
			set => this._NoComponent = value.FromEntia(this.World);
		}
		public global::Entia.Entity HasUnity
		{
			get => this._HasUnity.ToEntia();
			set => this._HasUnity = value.FromEntia(this.World);
		}
		public global::Entia.Entity NoUnity
		{
			get => this._NoUnity.ToEntia();
			set => this._NoUnity = value.FromEntia(this.World);
		}
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(HasComponent))] [global::Entia.Queryables.AllAttribute(new global::System.Type[] { typeof(Entia.IComponent) })]
		global::Entia.Unity.EntityReference _HasComponent;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(NoComponent))] [global::Entia.Queryables.NoneAttribute(new global::System.Type[] { typeof(Entia.IComponent) })]
		global::Entia.Unity.EntityReference _NoComponent;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(HasUnity))] [global::Entia.Queryables.AllAttribute(new global::System.Type[] { typeof(Entia.Components.Unity<>) })]
		global::Entia.Unity.EntityReference _HasUnity;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(NoUnity))] [global::Entia.Queryables.NoneAttribute(new global::System.Type[] { typeof(Entia.Components.Unity<>) })]
		global::Entia.Unity.EntityReference _NoUnity;
		public override global::Components.Link Component
		{
			get => new global::Components.Link
			{
				HasComponent = this.HasComponent,
				NoComponent = this.NoComponent,
				HasUnity = this.HasUnity,
				NoUnity = this.NoUnity
			};
			set
			{
				this.HasComponent = value.HasComponent;
				this.NoComponent = value.NoComponent;
				this.HasUnity = value.HasUnity;
				this.NoUnity = value.NoUnity;
			}
		}
	}
}