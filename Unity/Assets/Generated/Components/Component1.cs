using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Component1), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Component1" })][global::UnityEngine.AddComponentMenu("Components/Components.Component1")][global::UnityEngine.RequireComponent(typeof(global::UnityEngine.BoxCollider2D)), global::UnityEngine.RequireComponent(typeof(global::Components.Generated.IsFrozen)), global::UnityEngine.RequireComponent(typeof(global::Entia.Unity.EntityReference))]
	public sealed partial class Component1 : global::Entia.Unity.ComponentReference<global::Components.Component1>
	{
		public global::System.Single X
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.X, this._X);
			set => base.Set((ref global::Components.Component1 data, global::System.Single state, global::Entia.World _) => data.X = state, value, ref this._X);
		}
		new public global::Entia.Unity.EntityReference Entity
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.Entity.FromEntia(world), this._Entity);
			set => base.Set((ref global::Components.Component1 data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.Entity = state.ToEntia(), value, ref this._Entity);
		}
		new public global::Entia.Unity.EntityReference World
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.World.FromEntia(world), this._World);
			set => base.Set((ref global::Components.Component1 data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.World = state.ToEntia(), value, ref this._World);
		}
		public global::System.Int32 Component
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.Component, this._Component);
			set => base.Set((ref global::Components.Component1 data, global::System.Int32 state, global::Entia.World _) => data.Component = state, value, ref this._Component);
		}
		public global::UnityEngine.BoxCollider2D Collider
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.Collider, this._Collider);
			set => base.Set((ref global::Components.Component1 data, global::UnityEngine.BoxCollider2D state, global::Entia.World _) => data.Collider = state, value, ref this._Collider);
		}
		public global::Components.Generated.IsFrozen Reference1
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.Reference1, this._Reference1);
			set => base.Set((ref global::Components.Component1 data, global::Components.Generated.IsFrozen state, global::Entia.World _) => data.Reference1 = state, value, ref this._Reference1);
		}
		public global::Entia.Unity.EntityReference Reference2
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.Reference2, this._Reference2);
			set => base.Set((ref global::Components.Component1 data, global::Entia.Unity.EntityReference state, global::Entia.World _) => data.Reference2 = state, value, ref this._Reference2);
		}
		public global::System.ValueTuple<global::System.Single, global::System.Single> Tuple
		{
			get => base.Get((ref global::Components.Component1 data, global::Entia.World world) => data.Tuple, this._Tuple);
			set => base.Set((ref global::Components.Component1 data, global::System.ValueTuple<global::System.Single, global::System.Single> state, global::Entia.World _) => data.Tuple = state, value, ref this._Tuple);
		}
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
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(Tuple))]
		global::System.ValueTuple<global::System.Single, global::System.Single> _Tuple;
		public override global::Components.Component1 Raw
		{
			get => new global::Components.Component1
			{
				X = this._X,
				Entity = this._Entity.ToEntia(),
				World = this._World.ToEntia(),
				Component = this._Component,
				Collider = this._Collider,
				Reference1 = this._Reference1,
				Reference2 = this._Reference2,
				Tuple = this._Tuple
			};
			set
			{
				this._X = value.X;
				this._Entity = value.Entity.FromEntia(base.World);
				this._World = value.World.FromEntia(base.World);
				this._Component = value.Component;
				this._Collider = value.Collider;
				this._Reference1 = value.Reference1;
				this._Reference2 = value.Reference2;
				this._Tuple = value.Tuple;
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