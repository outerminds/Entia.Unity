using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Component1), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Component1" })][global::UnityEngine.AddComponentMenu("Components/Components.Component1")][global::UnityEngine.RequireComponent(typeof(global::UnityEngine.BoxCollider2D)), global::UnityEngine.RequireComponent(typeof(global::Components.Generated.IsFrozen)), global::UnityEngine.RequireComponent(typeof(global::Entia.Unity.EntityReference))]
	public sealed partial class Component1 : global::Entia.Unity.ComponentReference<global::Components.Component1>
	{
		ref global::System.Single X => ref this._X;
		new global::Entia.Entity Entity
		{
			get => this._Entity.ToEntia();
			set => this._Entity = value.FromEntia(base.World);
		}
		new global::Entia.Entity World
		{
			get => this._World.ToEntia();
			set => this._World = value.FromEntia(base.World);
		}
		new ref global::System.Int32 Component => ref this._Component;
		ref global::UnityEngine.BoxCollider2D Collider => ref this._Collider;
		ref global::Components.Generated.IsFrozen Reference1 => ref this._Reference1;
		ref global::Entia.Unity.EntityReference Reference2 => ref this._Reference2;
		ref global::System.Single Poulah => ref this._Poulah;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))] [global::UnityEngine.Serialization.FormerlySerializedAsAttribute("Poulah")] [global::Entia.Unity.RequireAttribute()]
		global::System.Single _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Entity))] [global::Entia.Unity.RequireAttribute()]
		global::Entia.Unity.EntityReference _Entity;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(World))] [global::Entia.Unity.RequireAttribute()]
		global::Entia.Unity.EntityReference _World;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Component))] [global::Entia.Unity.RequireAttribute()]
		global::System.Int32 _Component;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Collider))] [global::Entia.Unity.RequireAttribute()]
		global::UnityEngine.BoxCollider2D _Collider;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Reference1))] [global::Entia.Unity.RequireAttribute()]
		global::Components.Generated.IsFrozen _Reference1;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Reference2))] [global::Entia.Unity.RequireAttribute()]
		global::Entia.Unity.EntityReference _Reference2;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Poulah))]
		global::System.Single _Poulah;
		public override global::Components.Component1 Raw
		{
			get => new global::Components.Component1
			{
				X = this.X,
				Entity = this.Entity,
				World = this.World,
				Component = this.Component,
				Collider = this.Collider,
				Reference1 = this.Reference1,
				Reference2 = this.Reference2,
				Poulah = this.Poulah
			};
			set
			{
				this.X = value.X;
				this.Entity = value.Entity;
				this.World = value.World;
				this.Component = value.Component;
				this.Collider = value.Collider;
				this.Reference1 = value.Reference1;
				this.Reference2 = value.Reference2;
				this.Poulah = value.Poulah;
			}
		}
		protected override void Reset()
		{
			base.Reset();
			this.Collider = this.GetComponent<global::UnityEngine.BoxCollider2D>();
			this.Reference1 = this.GetComponent<global::Components.Generated.IsFrozen>();
			this.Reference2 = this.GetComponent<global::Entia.Unity.EntityReference>();
		}
	}
}