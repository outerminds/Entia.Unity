using Entia.Unity.Generation;

namespace Components.Inner.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Inner.Component3), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Inner", "Component3" })]
	[global::UnityEngine.AddComponentMenu("Components/Inner/Components.Inner.Component3")]
	public sealed partial class Component3 : global::Entia.Unity.ComponentReference<global::Components.Inner.Component3>
	{
		public ref global::System.Double X => ref this._X;
		public ref global::System.Double A => ref this._A;
		public ref global::System.Double B => ref this._B;
		public ref global::System.Double C => ref this._C;
		public ref global::System.Double D => ref this._D;
		public ref global::System.Double E => ref this._E;
		public ref global::System.Double F => ref this._F;
		public ref global::System.Double G => ref this._G;
		public ref global::System.Double H => ref this._H;
		public ref global::System.Double I => ref this._I;
		public ref global::System.Double J => ref this._J;
		public ref global::System.Double K => ref this._K;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))]
		global::System.Double _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(A))]
		global::System.Double _A;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(B))]
		global::System.Double _B;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(C))]
		global::System.Double _C;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(D))]
		global::System.Double _D;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(E))]
		global::System.Double _E;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(F))]
		global::System.Double _F;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(G))]
		global::System.Double _G;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(H))]
		global::System.Double _H;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(I))]
		global::System.Double _I;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(J))]
		global::System.Double _J;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(K))]
		global::System.Double _K;
		public override global::Components.Inner.Component3 Component
		{
			get => new global::Components.Inner.Component3
			{
				X = this.X,
				A = this.A,
				B = this.B,
				C = this.C,
				D = this.D,
				E = this.E,
				F = this.F,
				G = this.G,
				H = this.H,
				I = this.I,
				J = this.J,
				K = this.K
			};
			set
			{
				this.X = value.X;
				this.A = value.A;
				this.B = value.B;
				this.C = value.C;
				this.D = value.D;
				this.E = value.E;
				this.F = value.F;
				this.G = value.G;
				this.H = value.H;
				this.I = value.I;
				this.J = value.J;
				this.K = value.K;
			}
		}
	}
}