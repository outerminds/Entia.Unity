using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Component1), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Component1" })][global::UnityEngine.AddComponentMenu("Components/Components.Component1")][global::UnityEngine.RequireComponent(typeof(global::UnityEngine.Collider2D))]
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
		ref global::UnityEngine.Collider2D Collider => ref this._Collider;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))] [global::UnityEngine.Serialization.FormerlySerializedAsAttribute("Poulah")]
		global::System.Single _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Entity))]
		global::Entia.Unity.EntityReference _Entity;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(World))]
		global::Entia.Unity.EntityReference _World;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Component))]
		global::System.Int32 _Component;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Collider))] [global::Entia.Unity.RequireAttribute()]
		global::UnityEngine.Collider2D _Collider;
		public override global::Components.Component1 Raw
		{
			get => new global::Components.Component1
			{
				X = this.X,
				Entity = this.Entity,
				World = this.World,
				Component = this.Component,
				Collider = this.Collider
			};
			set
			{
				this.X = value.X;
				this.Entity = value.Entity;
				this.World = value.World;
				this.Component = value.Component;
				this.Collider = value.Collider;
			}
		}
		protected override void Reset()
		{
			base.Reset();
			this.Collider = this.GetComponent<global::UnityEngine.Collider2D>();
		}
	}
}