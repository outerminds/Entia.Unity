using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Component1), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Component1" })][global::UnityEngine.AddComponentMenu("Components/Components.Component1")][global::UnityEngine.RequireComponent(typeof(global::UnityEngine.BoxCollider2D)), global::UnityEngine.RequireComponent(typeof(global::Components.Generated.IsFrozen)), global::UnityEngine.RequireComponent(typeof(global::Entia.Unity.EntityReference))]
	public sealed partial class Component1 : global::Entia.Unity.ComponentReference<global::Components.Component1>
	{
		public ref global::System.Single X => ref base.Get((ref global::Components.Component1 data) => ref data.X, ref this._X);
		public global::Entia.Unity.EntityReference Entity
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.Entity.FromEntia(world), this._Entity);
			set => base.Set((ref global::Components.Component1 data, in global::Entia.Unity.EntityReference state, global::Entia.World _) => data.Entity = state.ToEntia(), value, ref this._Entity);
		}
		public global::Entia.Unity.EntityReference World
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.World.FromEntia(world), this._World);
			set => base.Set((ref global::Components.Component1 data, in global::Entia.Unity.EntityReference state, global::Entia.World _) => data.World = state.ToEntia(), value, ref this._World);
		}
		public ref global::System.Int32 Component => ref base.Get((ref global::Components.Component1 data) => ref data.Component, ref this._Component);
		public ref global::UnityEngine.BoxCollider2D Collider => ref base.Get((ref global::Components.Component1 data) => ref data.Collider, ref this._Collider);
		public ref global::Components.Generated.IsFrozen Reference1 => ref base.Get((ref global::Components.Component1 data) => ref data.Reference1, ref this._Reference1);
		public ref global::Entia.Unity.EntityReference Reference2 => ref base.Get((ref global::Components.Component1 data) => ref data.Reference2, ref this._Reference2);
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
		protected override global::Components.Component1 Raw
		{
			get => new global::Components.Component1
			{
				X = this._X,
				Entity = this._Entity.ToEntia(),
				World = this._World.ToEntia(),
				Component = this._Component,
				Collider = this._Collider,
				Reference1 = this._Reference1,
				Reference2 = this._Reference2
			};
			set
			{
				this._X = value.X;
				this._Entity = value.Entity.FromEntia(base._world);
				this._World = value.World.FromEntia(base._world);
				this._Component = value.Component;
				this._Collider = value.Collider;
				this._Reference1 = value.Reference1;
				this._Reference2 = value.Reference2;
			}
		}
		void Reset()
		{
			this.Collider = this.GetComponent<global::UnityEngine.BoxCollider2D>();
			this.Reference1 = this.GetComponent<global::Components.Generated.IsFrozen>();
			this.Reference2 = this.GetComponent<global::Entia.Unity.EntityReference>();
		}
	}
}