using Entia.Core;
using Entia.Unity.Generation;

namespace Components.Inner.Generated
{

	[global::Entia.Unity.Generation.GeneratedAttribute(Type = typeof(global::Components.Inner.Component3), Link = "Assets/Scripts/TestComponents.cs", Path = new string[] { "Components", "Inner", "Component3" })][global::UnityEngine.AddComponentMenu("Components/Inner/Components.Inner.Component3")]
	public sealed partial class Component3 : global::Entia.Unity.ComponentReference<global::Components.Inner.Component3>
	{
		public ref global::System.Double X => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.X, ref this._X);
		public ref global::System.Double A => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.A, ref this._A);
		public ref global::System.Double B => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.B, ref this._B);
		public ref global::System.Double C => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.C, ref this._C);
		public ref global::System.Double D => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.D, ref this._D);
		public ref global::System.Double E => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.E, ref this._E);
		public ref global::System.Double F => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.F, ref this._F);
		public ref global::System.Double G => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.G, ref this._G);
		public ref global::System.Double H => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.H, ref this._H);
		public ref global::System.Double I => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.I, ref this._I);
		public ref global::System.Double J => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.J, ref this._J);
		public ref global::System.Double K => ref base.Get((ref global::Components.Inner.Component3 data) => ref data.K, ref this._K);
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
		public override global::Components.Inner.Component3 Raw
		{
			get => new global::Components.Inner.Component3
			{
				X = this._X,
				A = this._A,
				B = this._B,
				C = this._C,
				D = this._D,
				E = this._E,
				F = this._F,
				G = this._G,
				H = this._H,
				I = this._I,
				J = this._J,
				K = this._K
			};
			set
			{
				this._X = value.X;
				this._A = value.A;
				this._B = value.B;
				this._C = value.C;
				this._D = value.D;
				this._E = value.E;
				this._F = value.F;
				this._G = value.G;
				this._H = value.H;
				this._I = value.I;
				this._J = value.J;
				this._K = value.K;
			}
		}

	}
}