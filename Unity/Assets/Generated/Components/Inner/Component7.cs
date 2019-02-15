using Entia.Unity.Generation;

namespace Components.Inner.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Inner.Component7), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Inner", "Component7" })][global::UnityEngine.AddComponentMenu("Components/Inner/Components.Inner.Component7")]
	public sealed partial class Component7 : global::Entia.Unity.ComponentReference<global::Components.Inner.Component7>
	{
		public ref global::System.Single X => ref this._X;
		public ref global::System.Single A => ref this._A;
		public ref global::System.Single B => ref this._B;
		public ref global::System.Single C => ref this._C;
		public ref global::System.Single D => ref this._D;
		public ref global::System.Single E => ref this._E;
		public ref global::System.Single F => ref this._F;
		public ref global::System.Single G => ref this._G;
		public ref global::System.Single H => ref this._H;
		public ref global::System.Single I => ref this._I;
		public ref global::System.Single J => ref this._J;
		public ref global::System.Single K => ref this._K;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))]
		global::System.Single _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(A))]
		global::System.Single _A;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(B))]
		global::System.Single _B;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(C))]
		global::System.Single _C;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(D))]
		global::System.Single _D;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(E))]
		global::System.Single _E;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(F))]
		global::System.Single _F;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(G))]
		global::System.Single _G;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(H))]
		global::System.Single _H;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(I))]
		global::System.Single _I;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(J))]
		global::System.Single _J;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(K))]
		global::System.Single _K;
		public override global::Components.Inner.Component7 Component
		{
			get => new global::Components.Inner.Component7
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