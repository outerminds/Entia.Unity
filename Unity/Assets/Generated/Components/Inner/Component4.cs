using Entia.Unity.Generation;

namespace Components.Inner.Generated
{
	using System.Linq;

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Inner.Component4), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Inner", "Component4" })][global::UnityEngine.AddComponentMenu("Components/Inner/Components.Inner.Component4")]
	public sealed partial class Component4 : global::Entia.Unity.ComponentReference<global::Components.Inner.Component4>
	{
		public ref global::System.UInt32 X => ref this._X;
		public ref global::System.UInt32 A => ref this._A;
		public ref global::System.UInt32 B => ref this._B;
		public ref global::System.UInt32 C => ref this._C;
		public ref global::System.UInt32 D => ref this._D;
		public ref global::System.UInt32 E => ref this._E;
		public ref global::System.UInt32 F => ref this._F;
		public ref global::System.UInt32 G => ref this._G;
		public ref global::System.UInt32 H => ref this._H;
		public ref global::System.UInt32 I => ref this._I;
		public ref global::System.UInt32 J => ref this._J;
		public ref global::System.UInt32 K => ref this._K;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(X))]
		global::System.UInt32 _X;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(A))]
		global::System.UInt32 _A;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(B))]
		global::System.UInt32 _B;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(C))]
		global::System.UInt32 _C;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(D))]
		global::System.UInt32 _D;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(E))]
		global::System.UInt32 _E;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(F))]
		global::System.UInt32 _F;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(G))]
		global::System.UInt32 _G;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(H))]
		global::System.UInt32 _H;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(I))]
		global::System.UInt32 _I;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(J))]
		global::System.UInt32 _J;
		[global::UnityEngine.SerializeField, global::UnityEngine.Serialization.FormerlySerializedAsAttribute(nameof(K))]
		global::System.UInt32 _K;
		public override global::Components.Inner.Component4 Component
		{
			get => new global::Components.Inner.Component4
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